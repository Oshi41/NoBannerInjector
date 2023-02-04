#pragma once
#include <fstream>
#include <iostream>
#include <sstream>
#include <vector>
#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS

EXTERN_C IMAGE_DOS_HEADER __ImageBase;
const LPWSTR AppPath = new WCHAR[_MAX_PATH];
static std::wstring file;
static std::wstring DllName;
static std::wstring Class;
static std::wstring Method;
static std::wstring Param;
static std::wstring ClrVersion;

#pragma comment(lib, "mscoree.lib")

void _Log(std::string str)
{
    time_t currTime;
    tm* currTm;
    time(&currTime);
    currTm = localtime(&currTime);
    char dateString1[50];
    strftime(dateString1, 50, "%d.%m.%Y %H:%M:%S", currTm);

    str = std::string(dateString1) + " " + str + "\n";

    std::cout << str;

    if (!file.empty())
    {
        std::ofstream s(file, std::ios_base::app | std::ios_base::out);
        s << str;
        s.close();
    }
}

void _Log(std::wstring s)
{
    _Log(std::string(s.begin(), s.end()));
}

void _Log(LPWSTR str)
{
    _Log(std::wstring(str));
}


void ShowMessage(std::string msg)
{
    MessageBoxA(nullptr, msg.c_str(), "Title", 0);
}

void ShowMessage(std::wstring msg)
{
    MessageBoxW(nullptr, msg.c_str(), L"Title", 0);
}


std::wstring read(std::wstring file)
{
    std::wifstream in;
    std::wstring text;
    std::wstring s;
    in.open(file, std::ios::in);
    while (getline(in, s))
    {
        text.append(s);
        text.append(L"\n");
    }
    return text;
}

std::wstring getValueFromJson(const std::wstring json, const std::wstring token)
{
    std::vector<std::wstring> strings;
    std::wistringstream f(token);
    std::wstring s;
    while (getline(f, s, L'.'))
        strings.push_back(s);

    std::size_t startPos = 0;

    for (auto _token : strings)
    {
        startPos = json.find(L"\"" + _token + L"\"", startPos);
        if (startPos == std::string::npos)
            return L"";
    }

    startPos = json.find(L":", startPos) + 1;
    std::size_t endPos = json.find(L",", startPos);
    if (endPos == std::string::npos)
        endPos = json.find(L"}", startPos);

    std::wstring res = json.substr(startPos + 1, endPos - startPos - 1);

    startPos = res.find(L"\"");
    if (startPos != std::string::npos)
    {
        endPos = res.find_last_of(L"\"");
        res = res.substr(startPos + 1, endPos - startPos - 1);
    }
    return res;
}


bool CheckResult(HRESULT res, std::string err)
{
    if (FAILED(res))
    {
        _Log(err);
        return false;
    }

    return true;
}


bool LoadConfig(HMODULE* lpParam)
{
    try
    {
        TCHAR moduleName[MAX_PATH];
        if (FAILED(GetModuleFileName(*lpParam, moduleName, MAX_PATH)))
        {
            _Log("Cannot get Module name");
            return false;
        }

        std::wstring parent_folder = moduleName;
        int index = parent_folder.find_last_of('\\');
        parent_folder.erase(index);

        auto log_dir_path = parent_folder + L"\\logs";
        CreateDirectory(log_dir_path.c_str(), NULL);
        file = log_dir_path + L"\\bootstrap.log";

        auto settings_path = parent_folder + L"\\settings\\cfg.json";
        auto json = read(settings_path);

        DllName = parent_folder + L"\\" + getValueFromJson(json, L"launch.c#.dll");
        Class = getValueFromJson(json, L"launch.c#.class");
        Method = getValueFromJson(json, L"launch.c#.method");
        Param = getValueFromJson(json, L"launch.c#.param");
        ClrVersion = getValueFromJson(json, L"launch.c#.clr");

        _Log(L"\n\n\n\nNEW START, Settings:\nDll - " + DllName
            + L"\nClass - " + Class + L"\nMethod - " + Method
            + L"\nParam - " + Param + L"\nCLR version - " + ClrVersion);

        return true;
    }
    catch (...)
    {
        _Log("Exception");
        ShowMessage("Exception");
        return false;
    }
}
