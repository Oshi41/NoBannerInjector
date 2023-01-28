#include <Windows.h>
#include <metahost.h>
#include <fstream>
#include <mscoree.h>
#include <wchar.h>

#pragma comment(lib, "mscoree.lib")

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

DWORD WINAPI CreateDotNetRunTime(LPVOID lpParam)
{
    static const char* Log = "Bootstrap.lib.log";
    static const LPCWSTR Assembly = L"\\CodeInject.dll";
    static const LPCWSTR Class = L"CodeInject.Startup";
    static const LPCWSTR Method = L"EntryPoint";
    static const LPCWSTR Param = L"";

    ICLRRuntimeHost* lpRuntimeHost = NULL;
    ICLRRuntimeInfo* lpRuntimeInfo = NULL;
    ICLRMetaHost* lpMetaHost = NULL;
    FILE* file;

    LPWSTR AppPath = new WCHAR[_MAX_PATH];
    ::GetModuleFileNameW((HINSTANCE)&__ImageBase, AppPath, _MAX_PATH);

    std::wstring tempPath = AppPath;
    int index = tempPath.rfind('\\');
    tempPath.erase(index, tempPath.length() - index);
    tempPath += Assembly;

    fopen_s(&file, Log, "a+");

    HRESULT metaHost = CLRCreateInstance(
        CLSID_CLRMetaHost,
        IID_ICLRMetaHost,
        (LPVOID*)&lpMetaHost
    );

    if (FAILED(metaHost))
    {
        fprintf(file, "Failed to create CLR instance.\n");
        fflush(file);
    }

    metaHost = lpMetaHost->GetRuntime(
        L"v4.0.30319",
        IID_PPV_ARGS(&lpRuntimeInfo)
    );

    if (FAILED(metaHost))
    {
        fprintf(file, "Getting runtime failed.\n");
        fflush(file);

        lpMetaHost->Release();
    }

    BOOL fLoadable;
    metaHost = lpRuntimeInfo->IsLoadable(&fLoadable);

    if (FAILED(metaHost) || !fLoadable)
    {
        fprintf(file, "Runtime can't be loaded into the process.\n");
        fflush(file);

        lpRuntimeInfo->Release();
        lpMetaHost->Release();
    }

    metaHost = lpRuntimeInfo->GetInterface(
        CLSID_CLRRuntimeHost,
        IID_PPV_ARGS(&lpRuntimeHost)
    );

    if (FAILED(metaHost))
    {
        fprintf(file, "Failed to acquire CLR runtime.\n");
        fflush(file);

        lpRuntimeInfo->Release();
        lpMetaHost->Release();
    }

    metaHost = lpRuntimeHost->Start();

    if (FAILED(metaHost))
    {
        fprintf(file, "Failed to start CLR runtime.\n");
        fflush(file);

        lpRuntimeHost->Release();
        lpRuntimeInfo->Release();
        lpMetaHost->Release();
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
        fprintf(file, "Unable to execute assembly.\n");
        fflush(file);

        lpRuntimeHost->Stop();
        lpRuntimeHost->Release();
        lpRuntimeInfo->Release();
        lpMetaHost->Release();
    }

    fclose(file);

    return 0;
}

DWORD APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
    switch (dwReason)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(NULL, NULL, CreateDotNetRunTime, NULL, NULL, NULL);
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
    default:
        break;
    }
    return true;
}
