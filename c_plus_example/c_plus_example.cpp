#include <chrono>
#include <string>
#include <fstream>
#include <iostream>
#include <ctime>
#include <iomanip>
#include <iostream>
#include <vector>
#include <sstream>

#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS


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

std::string timeStampToHReadble(const time_t rawtime)
{
    time_t currTime;
    tm* currTm;
    time(&currTime);
    currTm = localtime(&currTime);
    char dateString1[50];
    strftime(dateString1, 50, "%d.%m.%Y %H:%M:%S", currTm);
    return dateString1;
}

void Log(std::string string)
{
    time_t now = time(nullptr);
    auto formattedTime = timeStampToHReadble(now);

    std::cout << formattedTime << " " << string << std::endl;
    //
    // auto res = formattedTime._Fmtfirst + " " + string + "\n";
    // std::cout << string;
}

int main(int argc, char* argv[])
{
    auto path = L"C:/Users/oshi_/RiderProjects/Slash/bin/x64/Debug/settings/cfg.json";
    auto json = read(path);
    auto DllName = getValueFromJson(json, L"launch.c#.dll");
    auto Class = getValueFromJson(json, L"launch.c#.class");
    auto Method = getValueFromJson(json, L"launch.c#.method");
    auto Param = getValueFromJson(json, L"launch.c#.param");
    auto ClrVersion = getValueFromJson(json, L"launch.c#.clr");

    Log("1");
    Log("2");
    Log("3");
    Log("4");
    return 0;
}
