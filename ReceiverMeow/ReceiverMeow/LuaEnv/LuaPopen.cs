using System;
using System.Diagnostics;
using System.Text;

namespace ReceiverMeow.LuaEnv
{
    /// <summary>
    /// io.popen 兼容层
    /// KeraLua 1.4.x 的 Linux 原生库没有编译 popen，调用 io.popen 会直接抛
    /// "'popen' not supported"。这里在检测到原生不可用时，用托管代码补一个。
    /// 原生可用的平台（Windows/macOS）完全不受影响，仍然走原生实现。
    /// </summary>
    class LuaPopen
    {
        /// <summary>
        /// 原生 io.popen 是否可用，null表示还没检测过
        /// </summary>
        private static bool? nativeSupported = null;

        /// <summary>
        /// 检查并（必要时）安装替代实现
        /// </summary>
        public static void Install(NLua.Lua lua)
        {
            if (lua == null)
                return;
            if (nativeSupported == null)
                nativeSupported = Detect(lua);
            if (nativeSupported == true)
                return;

            try
            {
                lua["__meow_popen_read"] = new Func<string, string>(Read);
                lua["__meow_popen_write"] = new Func<string, string, int>(Write);
                lua.DoString(installScript);
                Log.Info("Lua", "当前lua原生库不支持io.popen，已启用内置替代实现");
            }
            catch (Exception e)
            {
                Log.Warn("Lua", $"安装io.popen替代实现失败：{e.Message}");
            }
        }

        /// <summary>
        /// 试一下原生popen能不能用
        /// </summary>
        private static bool Detect(NLua.Lua lua)
        {
            try
            {
                var r = lua.DoString(
                    "local ok = pcall(function() local f = io.popen('echo meow') if f then f:close() end end) return ok");
                return r != null && r.Length > 0 && Convert.ToBoolean(r[0]);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 执行命令，返回标准输出
        /// </summary>
        public static string Read(string cmd)
        {
            try
            {
                using var p = CreateProcess(cmd);
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                //统一成LF，跟C运行库文本模式下的popen行为一致，避免Windows上留下\r
                return (output ?? "").Replace("\r\n", "\n");
            }
            catch (Exception e)
            {
                Log.Warn("LuaPopen", $"执行命令失败：{cmd}，原因：{e.Message}");
                return "";
            }
        }

        /// <summary>
        /// 把输入喂给命令，返回退出码
        /// </summary>
        public static int Write(string cmd, string input)
        {
            try
            {
                using var p = CreateProcess(cmd);
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.StandardInput.Write(input ?? "");
                p.StandardInput.Close();
                p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return p.ExitCode;
            }
            catch (Exception e)
            {
                Log.Warn("LuaPopen", $"执行命令失败：{cmd}，原因：{e.Message}");
                return -1;
            }
        }

        private static Process CreateProcess(string cmd)
        {
            var p = new Process();
            var si = p.StartInfo;
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            si.RedirectStandardOutput = true;
            if (OperatingSystem.IsWindows())
            {
                si.FileName = "cmd.exe";
                si.ArgumentList.Add("/c");
                si.ArgumentList.Add(cmd);
            }
            else
            {
                si.FileName = "/bin/sh";
                si.ArgumentList.Add("-c");
                si.ArgumentList.Add(cmd);
            }
            return p;
        }

        /// <summary>
        /// 用lua实现的假文件句柄，支持 read/write/lines/close/flush，够脚本日常用了
        /// </summary>
        private const string installScript = @"
function io.popen(cmd, mode)
    if type(cmd) ~= 'string' then
        return nil, 'bad argument #1 to \'popen\' (string expected)'
    end
    mode = mode or 'r'
    if mode:sub(1, 1) == 'w' then
        local buf = {}
        local h = {}
        function h:write(...)
            for i = 1, select('#', ...) do
                buf[#buf + 1] = tostring((select(i, ...)))
            end
            return self
        end
        function h:flush() return self end
        function h:close()
            __meow_popen_write(cmd, table.concat(buf))
            return true
        end
        function h:read() return nil end
        function h:lines() return function() return nil end end
        return h
    end

    local out = __meow_popen_read(cmd) or ''
    local pos, len = 0, #out
    local closed = false
    local h = {}

    local function read_once(fmt)
        if closed then return nil end
        local f = tostring(fmt or '*l')
        local c = f:sub(1, 1)
        if c == '*' then c = f:sub(2, 2):lower() end
        if c == 'a' then
            --读到末尾也返回空串而不是nil，跟原生popen一致
            local v = out:sub(pos + 1)
            pos = len
            return v
        end
        if pos >= len then return nil end
        if type(fmt) == 'number' then
            local v = out:sub(pos + 1, pos + fmt)
            pos = pos + #v
            if v == '' then return nil end
            return v
        end
        if c == 'l' then
            local s, e = out:find('\n', pos + 1, true)
            if not s then return nil end
            local v = out:sub(pos + 1, s - 1)
            pos = e
            return v
        elseif c == 'L' then
            local s, e = out:find('\n', pos + 1, true)
            if not s then return nil end
            local v = out:sub(pos + 1, e)
            pos = e
            return v
        elseif c == 'n' then
            local s, e, num = out:find('^%s*(-?%d+%.?%d*)', pos + 1)
            if not s then return nil end
            pos = e
            return tonumber(num)
        end
        error('bad argument #1 to \'read\' (invalid format)', 2)
    end

    function h:read(...)
        local n = select('#', ...)
        if n == 0 then return read_once('*l') end
        local res = {}
        for i = 1, n do
            res[i] = read_once((select(i, ...)))
        end
        return table.unpack(res, 1, n)
    end
    function h:lines()
        return function()
            if closed then return nil end
            return read_once('*l')
        end
    end
    function h:close()
        closed = true
        return true
    end
    function h:flush() return true end
    return h
end
";
    }
}
