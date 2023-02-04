#include <Windows.h>
#include <metahost.h>
#include <fstream>
#include <mscoree.h>
#include <wchar.h>
#include <iostream>
#include <fstream>
#include <psapi.h>
#include "SimpleJSON/json.hpp"

#pragma comment(lib, "mscoree.lib")

EXTERN_C IMAGE_DOS_HEADER __ImageBase;
static const char* Log = "Bootstrap.dll.log";
static FILE* file;
static LPCWSTR DllName = L"CheatLib.dll";
static LPCWSTR Class = L"CheatLib.Startup";
static LPCWSTR Method = L"Main";
static LPCWSTR Param = L"";
static LPCWSTR ClrVersion = L"v4.0.30319";
const LPWSTR AppPath = new WCHAR[_MAX_PATH];

void _Log(std::string str)
{
    if (file == nullptr)
    {
        fopen_s(&file, Log, "a+");
    }
    fprintf(file, str.c_str());
    fprintf(file, "\n");
    fflush(file);
}

void _Log(LPWSTR str)
{
    std::wstring s(str);
    _Log(std::string(s.begin(), s.end()));
}

LPCWSTR ToStr(json::JSON json, LPCWSTR defaultVal)
{
    try
    {
        auto s = json.ToString();
        if (!s.empty())
        {
            const auto ws = std::wstring(s.begin(), s.end());
            return ws.c_str();
        }
    }
    catch (std::exception e)
    {
        _Log(e.what());
    }
    return defaultVal;
}

void LoadConfig()
{
    json::JSON launch_settings;
    try
    {
        std::ifstream in("cfg.json");
        const std::string contents((std::istreambuf_iterator<char>(in)), std::istreambuf_iterator<char>());
        auto settings = json::JSON::Load(contents);
        launch_settings = settings["launch"]["c#"];
        _Log("Configuration loaded");
    }
    catch (std::exception e)
    {
        _Log(e.what());
        launch_settings = json::JSON::Load("{}");
    }

    DllName = ToStr(launch_settings["dll"], DllName);
    Class = ToStr(launch_settings["class"], Class);
    Method = ToStr(launch_settings["method"], Method);
    Param = ToStr(launch_settings["argument"], Param);
    ClrVersion = ToStr(launch_settings["clr"], ClrVersion);
}

DWORD WINAPI CreateDotNetRunTime(HMODULE* lpParam)
{
    CoInitialize(NULL);
    AllocConsole();
    
    LPWSTR AppPath = new WCHAR[_MAX_PATH];
    ::GetModuleFileNameW((HINSTANCE)&__ImageBase, AppPath, _MAX_PATH);
    
    ICLRRuntimeHost* lpRuntimeHost = NULL;
    ICLRRuntimeInfo* lpRuntimeInfo = NULL;
    ICLRMetaHost* lpMetaHost = NULL;
    
    std::wstring tempPath = AppPath;
    int index = tempPath.rfind('\\');
    tempPath.erase(index, tempPath.length() - index);
    tempPath += L"\\";
    tempPath += DllName;
    
    HRESULT metaHost = CLRCreateInstance(
        CLSID_CLRMetaHost,
        IID_ICLRMetaHost,
        (LPVOID*)&lpMetaHost
    );
    
    if (FAILED(metaHost))
    {
        _Log("Failed to create CLR instance");
        return 1;
    }
    
    metaHost = lpMetaHost->GetRuntime(ClrVersion, IID_PPV_ARGS(&lpRuntimeInfo));
    
    if (FAILED(metaHost))
    {
        lpMetaHost->Release();
        _Log("Getting runtime failed");
        return 2;
    }
    
    BOOL fLoadable;
    metaHost = lpRuntimeInfo->IsLoadable(&fLoadable);
    
    if (FAILED(metaHost) || !fLoadable)
    {
        lpRuntimeInfo->Release();
        lpMetaHost->Release();
        _Log("Runtime can't be loaded into the process");
        return 3;
    }
    
    metaHost = lpRuntimeInfo->GetInterface(
        CLSID_CLRRuntimeHost,
        IID_PPV_ARGS(&lpRuntimeHost)
    );
    
    if (FAILED(metaHost))
    {
        lpRuntimeInfo->Release();
        lpMetaHost->Release();
        _Log("Failed to acquire CLR runtime");
        return 4;
    }
    
    metaHost = lpRuntimeHost->Start();
    
    if (FAILED(metaHost))
    {
        lpRuntimeHost->Release();
        lpRuntimeInfo->Release();
        lpMetaHost->Release();
        _Log("Failed to start CLR runtime");
        return 5;
    }
    
    DWORD dwRetCode = 0;
    
    metaHost = lpRuntimeHost->ExecuteInDefaultAppDomain(
        (LPWSTR)tempPath.c_str(),
        Class,
        Method,
        Param,
        &dwRetCode
    );    
    
    if (FAILED(metaHost))
    {
        lpRuntimeHost->Stop();        
        _Log("Unable to execute assembly");
        return 6;
    }
    
    lpRuntimeHost->Release();
    lpRuntimeInfo->Release();
    lpMetaHost->Release();
    
    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
    switch (dwReason)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)CreateDotNetRunTime, new HMODULE(hModule), 0, NULL);
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
    default:
        break;
    }
    return true;
}
