#include <Windows.h>
#include "utils.h"
#include <metahost.h>

DWORD WINAPI CreateClrRuntime(HMODULE* lpParam)
{
    AllocConsole();
    CoInitialize(NULL);

    if (!LoadConfig(lpParam))
    {
        _Log("Cannot load config");
        return 1;
    }

    ICLRMetaHost* meta = NULL;
    ICLRRuntimeInfo* info = NULL;
    ICLRRuntimeHost* host = NULL;
    BOOL fLoadable;
    DWORD dwRetCode = 0;

    HRESULT result = CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&meta);
    if (!CheckResult(result, "Cannot create CLR instance"))
        return 1;

    _Log("CLR instance created");
    result = meta->GetRuntime(ClrVersion.c_str(), IID_PPV_ARGS(&info));
    if (!CheckResult(result, "Cannot get runtime"))
    {
        meta->Release();
        return 1;
    }

    _Log("Desired CLR runtime version is presented");
    result = info->IsLoadable(&fLoadable);
    if (FAILED(result) || !fLoadable)
    {
        _Log("Runtime can't be loaded into the process");
        info->Release();
        meta->Release();
        return 1;
    }

    _Log("Runtime can be loaded into process");
    result = info->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&host));
    if (!CheckResult(result, "Failed to acquire CLR runtime"))
    {
        info->Release();
        meta->Release();
        return 1;
    }

    _Log("Obtain CLR runtime");
    result = host->Start();
    if (!CheckResult(result, "Failed to start CLR runtime"))
    {
        info->Release();
        meta->Release();
        host->Release();
        return 1;
    }

    _Log("Started CLR runtime");
    result = host->ExecuteInDefaultAppDomain(DllName.c_str(), Class.c_str(), Method.c_str(), Param.c_str(), &dwRetCode);
    if (!CheckResult(result, "Failed to load assembly: " + dwRetCode))
    {
        host->Stop();

        info->Release();
        meta->Release();
        host->Release();
        return 1;
    }

    _Log("Executed assembly");
    info->Release();
    meta->Release();
    host->Release();

    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
    switch (dwReason)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)CreateClrRuntime, new HMODULE(hModule), 0, NULL);
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
    default:
        break;
    }
    return true;
}
