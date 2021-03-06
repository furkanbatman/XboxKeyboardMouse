﻿using ScpDriverInterface;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using XboxKeyboardMouse.Libs;

namespace XboxKeyboardMouse {
    class TranslateKeyboard {
        public static bool TRIGGER_LEFT_PRESSED = false;
        public static bool TRIGGER_RIGHT_PRESSED = false;


        public enum TriggerType { LeftTrigger, RightTrigger }

        public static void ClearAllDicts() {
            if (mapLeftStickY == null)
                 mapLeftStickY = new Dictionary<Key, short>();
            else mapLeftStickY.Clear();

            if (mapLeftStickX == null)
                 mapLeftStickX = new Dictionary<Key, short>();
            else mapLeftStickX.Clear();

            if (mapRightStickX == null)
                 mapRightStickX = new Dictionary<Key, short>();
            else mapRightStickX.Clear();

            if (mapRightStickY == null)
                 mapRightStickY = new Dictionary<Key, short>();
            else mapRightStickY.Clear();

            if (buttons == null)
                 buttons = new Dictionary<Key, X360Buttons>();
            else buttons.Clear();

            if (triggers == null)
                 triggers = new Dictionary<Key, TriggerType>();
            else triggers.Clear();
        }

        public static Dictionary<Key, short> mapLeftStickY;
        public static Dictionary<Key, short> mapLeftStickX;
        public static Dictionary<Key, short> mapRightStickX;
        public static Dictionary<Key, short> mapRightStickY;
        
        public static Dictionary<Key, X360Buttons> buttons = new Dictionary<Key, X360Buttons>();
        public static Dictionary<Key, TriggerType> triggers = new Dictionary<Key, TriggerType>();

        private static void KeyInput(X360Controller controller) {
            List<bool> btnStatus = new List<bool>();

            //bool tLeft  = false;
            //bool tRight = false;

            try {
                btnStatus.Clear();

                bool mouseDisabled = Program.ActiveConfig.Mouse_Eng_Type == 4;
                

                // -------------------------------------------------------------------------------
                //                           LEFT STICK, AXIS - X
                // -------------------------------------------------------------------------------
                btnStatus.Clear();
                foreach (KeyValuePair<Key, short> entry in mapLeftStickX) {
                    if (entry.Key == Key.None) continue;

                    bool v;
                    if (entry.Key == Key.Escape)
                        v = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v = Keyboard.IsKeyDown(entry.Key);

                    if (v) controller.LeftStickX = entry.Value;
                    btnStatus.Add(v);
                } if (!btnStatus.Contains(true) && (mouseDisabled || Program.ActiveConfig.Mouse_Is_RightStick))
                    controller.LeftStickX = 0;


                // -------------------------------------------------------------------------------
                //                           LEFT STICK, AXIS - Y
                // -------------------------------------------------------------------------------
                foreach (KeyValuePair<Key, short> entry in mapLeftStickY) {
                    bool v;
                    if (entry.Key == Key.Escape)
                        v = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v = Keyboard.IsKeyDown(entry.Key);

                    if (v) controller.LeftStickY = entry.Value;

                    btnStatus.Add(v);
                } if (!btnStatus.Contains(true) && (mouseDisabled || Program.ActiveConfig.Mouse_Is_RightStick))
                    controller.LeftStickY = 0;


                // -------------------------------------------------------------------------------
                //                           RIGHT STICK, AXIS - X
                // -------------------------------------------------------------------------------
                foreach (KeyValuePair<Key, short> entry in mapRightStickX) {
                    bool v;
                    if (entry.Key == Key.Escape)
                        v = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v = Keyboard.IsKeyDown(entry.Key);

                    if (v) controller.RightStickX = entry.Value;

                    btnStatus.Add(v);
                } if (!btnStatus.Contains(true) && (mouseDisabled || !Program.ActiveConfig.Mouse_Is_RightStick))
                    controller.RightStickX = 0;


                // -------------------------------------------------------------------------------
                //                           RIGHT STICK, AXIS - Y
                // -------------------------------------------------------------------------------
                foreach (KeyValuePair<Key, short> entry in mapRightStickY) {
                    bool v;
                    if (entry.Key == Key.Escape)
                        v = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v = Keyboard.IsKeyDown(entry.Key);

                    if (v) controller.RightStickY = entry.Value;

                    btnStatus.Add(v);
                } if (!btnStatus.Contains(true) && (mouseDisabled || !Program.ActiveConfig.Mouse_Is_RightStick))
                    controller.RightStickY = 0;
                
                
                // -------------------------------------------------------------------------------
                //                                MISC BUTTONS
                // -------------------------------------------------------------------------------
                foreach (KeyValuePair<Key, X360Buttons> entry in buttons) {
                    if (entry.Key == Key.None) continue;

                    bool v;
                    if (entry.Key == Key.Escape)
                         v = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v = Keyboard.IsKeyDown(entry.Key);

                    if (v)
                         controller.Buttons = controller.Buttons | entry.Value;
                    else controller.Buttons = controller.Buttons & ~entry.Value;
                }


                // -------------------------------------------------------------------------------
                //                                TRIGGERS
                // -------------------------------------------------------------------------------

                foreach (KeyValuePair<Key, TriggerType> entry in triggers) {
                    if (entry.Key == Key.None) continue;

                    bool v;
                    if (entry.Key == Key.Escape)
                         v  = Hooks.LowLevelKeyboardHook.EscapePressed;
                    else v  = Keyboard.IsKeyDown(entry.Key);

                    bool ir = entry.Value == TriggerType.RightTrigger;

                    if (v) {
                        if (ir)
                             controller.RightTrigger = 255;
                        else controller.LeftTrigger = 255;
                    } else {
                        if (!TranslateKeyboard.TRIGGER_RIGHT_PRESSED && ir)
                            controller.RightTrigger = 0;
                        else if (!TranslateKeyboard.TRIGGER_LEFT_PRESSED && ir)
                            controller.LeftTrigger = 0;
                    }
                }

                //if (!tLeft)       controller.LeftTrigger = 0;
                //else if (!tRight) controller.RightTrigger = 0;
            } catch (Exception ex) { /* This occures when changing presets */ }
        }

        private static void Debug_TimeTracer(X360Controller Controller) {
            // Get the start time
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Call the key input
            KeyInput(Controller);

            // Get the end time
            watch.Stop();
            string time = watch.ElapsedMilliseconds + " MS";
            
            if (time != "0 MS") {
                // Display the time
                Logger.appendLogLine("KeyboardInput", $"Timed @ {time}", Logger.Type.Debug);
            }
        }

        public static void KeyboardInput(X360Controller controller) =>
            #if (DEBUG)
                    // Only enable if you have timing issues AKA Latency on 
                    // the keyboard inputs
                    Debug_TimeTracer(controller);
                    //KeyInput(controller);
            #else
                    KeyInput(controller);
            #endif
    }
}
