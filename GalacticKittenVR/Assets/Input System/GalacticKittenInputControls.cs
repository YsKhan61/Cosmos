//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input System/GalacticKittenInputControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GalacticKittenInputControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GalacticKittenInputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GalacticKittenInputControls"",
    ""maps"": [
        {
            ""name"": ""SpaceshipActionMap"",
            ""id"": ""bd426e1b-4202-4750-aafa-58451e184342"",
            ""actions"": [
                {
                    ""name"": ""ThrottleForward"",
                    ""type"": ""Button"",
                    ""id"": ""49a24af5-b5f5-46c5-99b7-fd32082fc77c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrottleBackward"",
                    ""type"": ""Button"",
                    ""id"": ""8201062c-517b-4982-bcf2-70f3c7fcffe4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ControlHandle"",
                    ""type"": ""Value"",
                    ""id"": ""7af1ba11-ec51-49a2-a2e6-02a5bde3b841"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""89557b55-0d6c-48a7-a2b2-19e13bdf8032"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""ThrottleForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97b37590-17fd-4789-8357-9cfb45b95fd0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""ThrottleBackward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4003ecab-b03a-4fde-8e00-7097bba09a0d"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""ControlHandle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""MnK Control Schema"",
            ""bindingGroup"": ""MnK Control Schema"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // SpaceshipActionMap
        m_SpaceshipActionMap = asset.FindActionMap("SpaceshipActionMap", throwIfNotFound: true);
        m_SpaceshipActionMap_ThrottleForward = m_SpaceshipActionMap.FindAction("ThrottleForward", throwIfNotFound: true);
        m_SpaceshipActionMap_ThrottleBackward = m_SpaceshipActionMap.FindAction("ThrottleBackward", throwIfNotFound: true);
        m_SpaceshipActionMap_ControlHandle = m_SpaceshipActionMap.FindAction("ControlHandle", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // SpaceshipActionMap
    private readonly InputActionMap m_SpaceshipActionMap;
    private List<ISpaceshipActionMapActions> m_SpaceshipActionMapActionsCallbackInterfaces = new List<ISpaceshipActionMapActions>();
    private readonly InputAction m_SpaceshipActionMap_ThrottleForward;
    private readonly InputAction m_SpaceshipActionMap_ThrottleBackward;
    private readonly InputAction m_SpaceshipActionMap_ControlHandle;
    public struct SpaceshipActionMapActions
    {
        private @GalacticKittenInputControls m_Wrapper;
        public SpaceshipActionMapActions(@GalacticKittenInputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ThrottleForward => m_Wrapper.m_SpaceshipActionMap_ThrottleForward;
        public InputAction @ThrottleBackward => m_Wrapper.m_SpaceshipActionMap_ThrottleBackward;
        public InputAction @ControlHandle => m_Wrapper.m_SpaceshipActionMap_ControlHandle;
        public InputActionMap Get() { return m_Wrapper.m_SpaceshipActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SpaceshipActionMapActions set) { return set.Get(); }
        public void AddCallbacks(ISpaceshipActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Add(instance);
            @ThrottleForward.started += instance.OnThrottleForward;
            @ThrottleForward.performed += instance.OnThrottleForward;
            @ThrottleForward.canceled += instance.OnThrottleForward;
            @ThrottleBackward.started += instance.OnThrottleBackward;
            @ThrottleBackward.performed += instance.OnThrottleBackward;
            @ThrottleBackward.canceled += instance.OnThrottleBackward;
            @ControlHandle.started += instance.OnControlHandle;
            @ControlHandle.performed += instance.OnControlHandle;
            @ControlHandle.canceled += instance.OnControlHandle;
        }

        private void UnregisterCallbacks(ISpaceshipActionMapActions instance)
        {
            @ThrottleForward.started -= instance.OnThrottleForward;
            @ThrottleForward.performed -= instance.OnThrottleForward;
            @ThrottleForward.canceled -= instance.OnThrottleForward;
            @ThrottleBackward.started -= instance.OnThrottleBackward;
            @ThrottleBackward.performed -= instance.OnThrottleBackward;
            @ThrottleBackward.canceled -= instance.OnThrottleBackward;
            @ControlHandle.started -= instance.OnControlHandle;
            @ControlHandle.performed -= instance.OnControlHandle;
            @ControlHandle.canceled -= instance.OnControlHandle;
        }

        public void RemoveCallbacks(ISpaceshipActionMapActions instance)
        {
            if (m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ISpaceshipActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public SpaceshipActionMapActions @SpaceshipActionMap => new SpaceshipActionMapActions(this);
    private int m_MnKControlSchemaSchemeIndex = -1;
    public InputControlScheme MnKControlSchemaScheme
    {
        get
        {
            if (m_MnKControlSchemaSchemeIndex == -1) m_MnKControlSchemaSchemeIndex = asset.FindControlSchemeIndex("MnK Control Schema");
            return asset.controlSchemes[m_MnKControlSchemaSchemeIndex];
        }
    }
    public interface ISpaceshipActionMapActions
    {
        void OnThrottleForward(InputAction.CallbackContext context);
        void OnThrottleBackward(InputAction.CallbackContext context);
        void OnControlHandle(InputAction.CallbackContext context);
    }
}