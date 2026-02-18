using OpenGM.Rendering;
using OpenGM.VirtualMachine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;

namespace OpenGM.IO;

public class InputHandler
{
    public enum State
    {
        NORMAL,
        PLAYBACK,
        RECORD
    }

    public static readonly byte[] ReplayHeader = "OGMR"u8.ToArray();

    public static State HandlerState = State.NORMAL;
    public static Stream? IOStream;

    public static bool[] KeyDown = new bool[256];
    public static bool[] KeyPressed = new bool[256];
    public static bool[] KeyReleased = new bool[256];
    public static bool[] KeySuppressed = new bool[256];

    public static bool[] MouseDown = new bool[5];
    public static bool[] MousePressed = new bool[5];
    public static bool[] MouseReleased = new bool[5];

    public static Vector2 MousePos = new();

    public static string KeyboardString = "";

    // Gamepad state
    public const int MaxGamepads = 12;
    public const int MaxGamepadButtons = 20; // gp_face1 through gp_padr
    public const int GamepadAxisBase = 32785; // gp_axislh

    public static bool[] GamepadConnected = new bool[MaxGamepads];
    public static bool[,] GamepadButtonDown = new bool[MaxGamepads, MaxGamepadButtons];
    public static bool[,] GamepadButtonPressed = new bool[MaxGamepads, MaxGamepadButtons];
    public static bool[,] GamepadButtonReleased = new bool[MaxGamepads, MaxGamepadButtons];
    public static float[,] GamepadAxisValues = new float[MaxGamepads, 4];
    public static float[] GamepadAxisDeadzone = { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
    public static int[,] GamepadHatValues = new int[MaxGamepads, 4];
    public static string[] GamepadDescriptions = new string[MaxGamepads];

    public static void UpdateMouseState(MouseState state)
    {
        var mouseButtons = new[] { MouseButton.Left, MouseButton.Right, MouseButton.Middle, MouseButton.Button1, MouseButton.Button2 };

        for (var i = 0; i < 5; i++)
        {
            var isDown = state.IsButtonDown(mouseButtons[i]);
            var wasDown = MouseDown[i];

            MousePressed[i] = isDown && !wasDown;
            MouseReleased[i] = !isDown && wasDown;
            MouseDown[i] = isDown;
        }

        MousePos.X = state.X;
        MousePos.Y = state.Y;
    }

    public static void UpdateGamepadState(IReadOnlyList<JoystickState> joystickStates)
    {
        for (var device = 0; device < Math.Min(joystickStates.Count, MaxGamepads); device++)
        {
            var isPresent = GLFW.JoystickPresent(device);
            var joyState = joystickStates[device];

            if (!isPresent)
            {
                if (GamepadConnected[device])
                {
                    DebugLog.LogInfo($"Gamepad {device} disconnected.");
                    // Device disconnected - clear state
                    GamepadConnected[device] = false;
                    GamepadDescriptions[device] = "";
                    for (var b = 0; b < MaxGamepadButtons; b++)
                    {
                        GamepadButtonDown[device, b] = false;
                        GamepadButtonPressed[device, b] = false;
                        GamepadButtonReleased[device, b] = false;
                    }
                    for (var a = 0; a < 4; a++)
                    {
                        GamepadAxisValues[device, a] = 0;
                    }
                }
                continue;
            }

            var name = GLFW.GetJoystickName(device) ?? "Unknown Gamepad";

            if (!GamepadConnected[device])
            {
                DebugLog.LogInfo($"Gamepad {device} connected: {name} (Buttons: {joyState.ButtonCount}, Axes: {joyState.AxisCount}, Hats: {joyState.HatCount})");
            }

            GamepadConnected[device] = true;
            GamepadDescriptions[device] = name;

            // Update buttons
            for (var b = 0; b < joyState.ButtonCount; b++)
            {
                var isDown = b < joyState.ButtonCount && joyState.IsButtonDown(b);
                if (joyState.IsButtonDown(b)) Console.WriteLine($"Button {b} down");

                var wasDown = GamepadButtonDown[device, b];

                GamepadButtonPressed[device, b] = isDown && !wasDown;
                GamepadButtonReleased[device, b] = !isDown && wasDown;
                GamepadButtonDown[device, b] = isDown;
            }

            // Update axes (0=LH, 1=LV, 2=RH, 3=RV)
            for (var a = 0; a < 4; a++)
            {
                if (a < joyState.AxisCount)
                {
                    var value = joyState.GetAxis(a);
                    var deadzone = GamepadAxisDeadzone[device];
                    if (Math.Abs(value) < deadzone)
                    {
                        value = 0;
                    }
                    GamepadAxisValues[device, a] = value;
                }
                else
                {
                    GamepadAxisValues[device, a] = 0;
                }
            }

            // Update hats
            for (var h = 0; h < Math.Min(joyState.HatCount, 4); h++)
            {
                GamepadHatValues[device, h] = (int)joyState.GetHat(h);
            }
        }
    }

    public static int GetGamepadButtonIndex(int keyid)
    {
        return keyid switch
        {
            32769 => 0,  // gp_face1
            32770 => 1,  // gp_face2
            32771 => 2,  // gp_face3
            32772 => 3,  // gp_face4
            32773 => 4,  // gp_shoulderl
            32774 => 5,  // gp_shoulderr
            32777 => 9,  // gp_select
            32778 => 8,  // gp_start
            32779 => 11,  // gp_stickl
            32780 => 12, // gp_stickr
            32781 => 13, // gp_padu
            32782 => 15, // gp_padd
            32783 => 16, // gp_padl
            32784 => 14, // gp_padr
            _ => keyid - 32769
        };
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    /// most virtual keys map to opentk keys, but some dont, so we gotta do that here
    /// </summary>
    public static Keys[] Convert(int keyid)
    {
        return keyid switch
        {
            0x08 => [Keys.Backspace],
            0x0D => [Keys.Enter],
            0x10 => [Keys.LeftShift, Keys.RightShift],
            0x11 => [Keys.LeftControl, Keys.RightControl],
            0x12 => [Keys.LeftAlt, Keys.RightAlt],
            0x1B => [Keys.Escape],

            0x21 => [Keys.PageUp],
            0x22 => [Keys.PageDown],
            0x23 => [Keys.End],
            0x24 => [Keys.Home],
            0x25 => [Keys.Left],
            0x26 => [Keys.Up],
            0x27 => [Keys.Right],
            0x28 => [Keys.Down],
            0x2D => [Keys.Insert],
            0x2E => [Keys.Delete],

            0x70 => [Keys.F1],
            0x71 => [Keys.F2],
            0x72 => [Keys.F3],
            0x73 => [Keys.F4],
            0x74 => [Keys.F5],
            0x75 => [Keys.F6],
            0x76 => [Keys.F7],
            0x77 => [Keys.F8],
            0x78 => [Keys.F9],
            0x79 => [Keys.F10],
            0x7A => [Keys.F11],
            0x7B => [Keys.F12],

            0xA0 => [Keys.LeftShift],
            0xA1 => [Keys.RightShift],

            _ => [(Keys)keyid]
        };
    }

    private static void RecordIOState()
    {
        if (IOStream == null)
        {
            throw new NullReferenceException("IOStream is null.");
        }

        IOStream.Write(new byte[4]); // IO_LastChar

        // TODO: no idea if this matches the runner, but assuming that
        // it's a 4 byte length + 4096 byte buffer

        var inputBytes = Encoding.ASCII.GetBytes(KeyboardString);
        IOStream.Write(BitConverter.GetBytes(inputBytes.Length));

        Array.Resize(ref inputBytes, 4096);
        IOStream.Write(inputBytes);

        IOStream.Write(new byte[4]); // IO_LastKey
        IOStream.Write(new byte[4]); // IO_CurrentKey

        for (var i = 0; i < 256; i++)
        {
            IOStream.WriteByte(KeyDown[i] ? (byte)1 : (byte)0);
        }

        for (var i = 0; i < 256; i++)
        {
            IOStream.WriteByte(KeyReleased[i] ? (byte)1 : (byte)0);
        }

        for (var i = 0; i < 256; i++)
        {
            IOStream.WriteByte(KeyPressed[i] ? (byte)1 : (byte)0);
        }

        IOStream.Write(new byte[40]); // IO_LastButton
        IOStream.Write(new byte[40]); // IO_CurrentButton

        IOStream.Write(new byte[50]); // IO_ButtonDown
        IOStream.Write(new byte[50]); // IO_ButtonReleased
        IOStream.Write(new byte[50]); // IO_ButtonPressed

        IOStream.Write(new byte[10]); // IO_WheelUp
        IOStream.Write(new byte[10]); // IO_WheelDown

        IOStream.Write(new byte[8]); // IO_MousePos
        IOStream.Write(new byte[4]); // IO_MouseX
        IOStream.Write(new byte[4]); // IO_MouseY
    }

    private static void PlaybackIOState()
    {
        if (IOStream == null)
        {
            throw new NullReferenceException("IOStream is null.");
        }

        CustomWindow.Instance.UpdateFrequency = 0.0;

        try
        {
            IOStream.ReadExactly(new byte[4]); // IO_LastChar

            var inputLengthBuf = new byte[4];
            IOStream.ReadExactly(inputLengthBuf);
            var inputLength = BitConverter.ToInt32(inputLengthBuf);

            var inputBytes = new byte[4096];
            IOStream.ReadExactly(inputBytes);
            KeyboardString = Encoding.ASCII.GetString(inputBytes[..inputLength]);

            IOStream.ReadExactly(new byte[4]); // IO_LastKey
            IOStream.ReadExactly(new byte[4]); // IO_CurrentKey

            for (var i = 0; i < 256; i++)
            {
                KeyDown[i] = IOStream.ReadByte() == 1;
            }

            for (var i = 0; i < 256; i++)
            {
                KeyReleased[i] = IOStream.ReadByte() == 1;
            }

            for (var i = 0; i < 256; i++)
            {
                KeyPressed[i] = IOStream.ReadByte() == 1;
            }

            IOStream.ReadExactly(new byte[40]); // IO_LastButton
            IOStream.ReadExactly(new byte[40]); // IO_CurrentButton

            IOStream.ReadExactly(new byte[50]); // IO_ButtonDown
            IOStream.ReadExactly(new byte[50]); // IO_ButtonReleased
            IOStream.ReadExactly(new byte[50]); // IO_ButtonPressed

            IOStream.ReadExactly(new byte[10]); // IO_WheelUp
            IOStream.ReadExactly(new byte[10]); // IO_WheelDown

            IOStream.ReadExactly(new byte[8]); // IO_MousePos
            IOStream.ReadExactly(new byte[4]); // IO_MouseX
            IOStream.ReadExactly(new byte[4]); // IO_MouseY
        }
        catch (EndOfStreamException)
        {
            DebugLog.LogInfo("Hit EOF, ending replay.");
            CustomWindow.Instance.UpdateFrequency = Entry.GameSpeed;
            IOStream.Close();
            HandlerState = State.NORMAL;
        }
    }

    public static void UpdateKeyboardState(KeyboardState state)
    {
        if (HandlerState == State.PLAYBACK)
        {
            PlaybackIOState();
            return;
        }

        void CalculateKey(int vk, bool isDown)
        {
            if (KeySuppressed[vk])
            {
                if (isDown)
                {
                    isDown = false;
                }
                else
                {
                    KeySuppressed[vk] = false;
                }
            }

            var wasDown = KeyDown[vk];

            KeyPressed[vk] = isDown && !wasDown;
            KeyReleased[vk] = !isDown && wasDown;
            KeyDown[vk] = isDown;
        }

        CalculateKey(0, !state.IsAnyKeyDown); // vk_nokey
        CalculateKey(1, state.IsAnyKeyDown); // vk_anykey

        for (var i = 2; i < 256; i++)
        {
            CalculateKey(i, CustomWindow.Instance.IsFocused && Convert(i).Any(state.IsKeyDown));
        }

        // TODO: account for caps lock?
        for (var i = 'A'; i <= 'Z'; i++)
        {
            if (KeyPressed[i])
            {
                var chr = i;

                if (!KeyPressed[0x10])
                {
                    chr += (char)32;
                }

                KeyboardString += chr;
            }
        }

        if (KeyPressed[8] && KeyboardString.Length > 0)
        {
            // backspace
            KeyboardString = KeyboardString[..^1];
        }

        // debug
        VMExecutor.VerboseStackLogs = VMExecutor.ForceVerboseStackLogs || state.IsKeyDown(Keys.F1);

        if (state.IsKeyDown(Keys.F2))
        {
            CustomWindow.Instance.UpdateFrequency = 0.0; // This means fastest
        }
        else if (state.IsKeyDown(Keys.F3))
        {
            CustomWindow.Instance.UpdateFrequency = 2;
        }
        else
        {
            CustomWindow.Instance.UpdateFrequency = Entry.GameSpeed;
        }

        if (state.IsKeyPressed(Keys.F5))
        {
            VMExecutor.DebugMode = !VMExecutor.DebugMode;
            VariableResolver.GlobalVariables["debug"] = VMExecutor.DebugMode;
            DebugLog.LogInfo($"Debug mode : {VMExecutor.DebugMode}");
        }

        if (state.IsKeyPressed(Keys.F6))
        {
            TimingManager.DebugTime = !TimingManager.DebugTime;
        }

        if (state.IsKeyPressed(Keys.KeyPad0))
        {
            DebugLog.PrintInstances(DebugLog.LogType.Info);
            DebugLog.PrintInactiveInstances(DebugLog.LogType.Info);
            DebugLog.PrintDrawObjects(DebugLog.LogType.Info);
        }

        if (state.IsKeyPressed(Keys.KeyPad1))
        {
            DebugLog.Log("LAYERS :");
            foreach (var (id, layer) in RoomManager.CurrentRoom.Layers)
            {
                DebugLog.Log($" - Layer: {layer.Name} ({id})");
                foreach (var element in layer.ElementsToDraw)
                {
                    var str = $"     - {element.GetType().Name} ({element.instanceId})";
                    if (element is GMSprite sprite)
                    {

                        str += $" [{sprite.X}, {sprite.Y}]";
                    }
                    else if (element is GMBackground bg)
                    {

                        str += $" Index:{bg.Element.Index} Frame:{bg.FrameIndex}";
                    }
                    DebugLog.Log(str);
                }
            }
        }

        if (state.IsKeyPressed(Keys.KeyPad2))
        {
            DrawManager.DebugBBoxes = !DrawManager.DebugBBoxes;
            DebugLog.LogInfo($"Draw bounding boxes: {DrawManager.DebugBBoxes}");
        }

        if (state.IsKeyPressed(Keys.KeyPad3))
        {
            if (HandlerState == State.RECORD)
            {
                DebugLog.LogInfo("Finished recording.");
                HandlerState = State.NORMAL;
            }
        }

        if (state.IsKeyPressed(Keys.KeyPad4))
        {
            DrawManager.ShouldDrawGui = !DrawManager.ShouldDrawGui;
            DebugLog.LogInfo($"GUI rendering {(DrawManager.ShouldDrawGui ? "enabled" : "disabled")}.");
        }

        if (state.IsKeyPressed(Keys.KeyPad5))
        {
            GraphicsManager.EnableCulling = !GraphicsManager.EnableCulling;
            DebugLog.LogInfo($"Frustum culling {(GraphicsManager.EnableCulling ? "enabled" : "disabled")}.");
        }

        if (HandlerState == State.RECORD)
        {
            RecordIOState();
        }
    }

    public static bool KeyboardCheckDirect(int key)
    {
        // TODO : work out how this is different? tried using GetAsyncKeyState and it didnt work
        if (key < 256)
        {
            return KeyDown[key];
        }

        return false;
    }
}
