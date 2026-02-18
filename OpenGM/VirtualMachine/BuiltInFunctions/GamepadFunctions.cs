using OpenGM.IO;

namespace OpenGM.VirtualMachine.BuiltInFunctions
{
    public static class GamepadFunctions
    {
        [GMLFunction("gamepad_is_supported")]
        public static object gamepad_is_supported(object?[] args)
        {
            return true;
        }

        [GMLFunction("gamepad_get_device_count")]
        public static object gamepad_get_device_count(object?[] args)
        {
            return InputHandler.MaxGamepads;
        }

        [GMLFunction("gamepad_is_connected")]
        public static object gamepad_is_connected(object?[] args)
        {
            var device = args[0].Conv<int>();
            if (device < 0 || device >= InputHandler.MaxGamepads)
            {
                return false;
            }
            return InputHandler.GamepadConnected[device];
        }

        [GMLFunction("gamepad_get_description")]
        public static object gamepad_get_description(object?[] args)
        {
            var device = args[0].Conv<int>();
            if (device < 0 || device >= InputHandler.MaxGamepads || !InputHandler.GamepadConnected[device])
            {
                return "No Gamepad";
            }
            return InputHandler.GamepadDescriptions[device] ?? "Unknown Gamepad";
        }

        // gamepad_get_button_threshold
        // gamepad_set_button_threshold

        [GMLFunction("gamepad_get_axis_deadzone")]
        public static object gamepad_get_axis_deadzone(object?[] args)
        {
            var device = args[0].Conv<int>();
            if (device < 0 || device >= InputHandler.MaxGamepads)
            {
                return 0.2;
            }
            return (double)InputHandler.GamepadAxisDeadzone[device];
        }

        [GMLFunction("gamepad_set_axis_deadzone")]
        public static object? gamepad_set_axis_deadzone(object?[] args)
        {
            var device = args[0].Conv<int>();
            var deadzone = args[1].Conv<float>();
            if (device >= 0 && device < InputHandler.MaxGamepads)
            {
                InputHandler.GamepadAxisDeadzone[device] = deadzone;
            }
            return null;
        }

        [GMLFunction("gamepad_button_count")]
        public static object gamepad_button_count(object?[] args)
        {
            var device = args[0].Conv<int>();
            if (device < 0 || device >= InputHandler.MaxGamepads || !InputHandler.GamepadConnected[device])
            {
                return 0;
            }
            return InputHandler.MaxGamepadButtons;
        }

        [GMLFunction("gamepad_button_check")]
        public static object gamepad_button_check(object?[] args)
        {
            var device = args[0].Conv<int>();
            var button = args[1].Conv<int>();
            var index = InputHandler.GetGamepadButtonIndex(button);

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                index < 0 || index >= InputHandler.MaxGamepadButtons)
            {
                return false;
            }
            return InputHandler.GamepadButtonDown[device, index];
        }

        [GMLFunction("gamepad_button_check_pressed")]
        public static object gamepad_button_check_pressed(object?[] args)
        {
            var device = args[0].Conv<int>();
            var button = args[1].Conv<int>();
            var index = InputHandler.GetGamepadButtonIndex(button);

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                index < 0 || index >= InputHandler.MaxGamepadButtons)
            {
                return false;
            }

            return InputHandler.GamepadButtonPressed[device, index];
        }

        [GMLFunction("gamepad_button_check_released")]
        public static object gamepad_button_check_released(object?[] args)
        {
            var device = args[0].Conv<int>();
            var button = args[1].Conv<int>();
            var index = InputHandler.GetGamepadButtonIndex(button);

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                index < 0 || index >= InputHandler.MaxGamepadButtons)
            {
                return false;
            }

            return InputHandler.GamepadButtonReleased[device, index];
        }

        [GMLFunction("gamepad_button_value")]
        public static object gamepad_button_value(object?[] args)
        {
            var device = args[0].Conv<int>();
            var button = args[1].Conv<int>();
            var index = InputHandler.GetGamepadButtonIndex(button);

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                index < 0 || index >= InputHandler.MaxGamepadButtons)
            {
                return 0.0;
            }

            return InputHandler.GamepadButtonDown[device, index] ? 1.0 : 0.0;
        }

        [GMLFunction("gamepad_axis_count")]
        public static object gamepad_axis_count(object?[] args)
        {
            var device = args[0].Conv<int>();
            if (device < 0 || device >= InputHandler.MaxGamepads || !InputHandler.GamepadConnected[device])
            {
                return 0;
            }
            return 4;
        }

        [GMLFunction("gamepad_axis_value")]
        public static object gamepad_axis_value(object?[] args)
        {
            var device = args[0].Conv<int>();
            var axis = args[1].Conv<int>();
            var index = axis - InputHandler.GamepadAxisBase;

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                index < 0 || index >= 4)
            {
                return 0.0;
            }

            return (double)InputHandler.GamepadAxisValues[device, index];
        }

        [GMLFunction("gamepad_hat_value")]
        public static object gamepad_hat_value(object?[] args)
        {
            var device = args[0].Conv<int>();
            var hat = args[1].Conv<int>();

            if (device < 0 || device >= InputHandler.MaxGamepads ||
                hat < 0 || hat >= 4)
            {
                return 0;
            }

            return InputHandler.GamepadHatValues[device, hat];
        }

        // gamepad_hat_count
        // gamepad_remove_mapping
        // gamepad_test_mapping
        // gamepad_get_mapping
        // gamepad_get_guid

        [GMLFunction("gamepad_set_vibration", GMLFunctionFlags.Stub)]
        public static object? gamepad_set_vibration(object?[] args)
        {
            // OpenTK doesn't support rumble on all platforms
            return null;
        }

        // gamepad_add_hardware_mapping_from_string
        // gamepad_add_hardware_mapping_from_file
        // gamepad_get_hardware_mappings
        // gamepad_set_color
        // gamepad_set_colour
        // gamepad_set_option
        // gamepad_get_option
    }
}
