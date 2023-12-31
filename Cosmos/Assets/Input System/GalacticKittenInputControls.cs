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
                    ""name"": ""ThrottleHandleAction"",
                    ""type"": ""Value"",
                    ""id"": ""8d4ba86c-a9bd-4ac1-90a4-755f6a2aeaec"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ControlHandleAction"",
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
                    ""name"": ""1D Axis"",
                    ""id"": ""e916bfbc-ab07-4bc4-b801-86c10d066dcd"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleHandleAction"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""40e590c7-46f7-4f9c-b094-b8b7fec70b6e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""ThrottleHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a433925f-34ce-46aa-b2b2-59a4ff085f1f"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""ThrottleHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector MnK"",
                    ""id"": ""133eb049-7e63-489f-a201-101b49a168a6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControlHandleAction"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5f5c488e-2c6a-4645-b84c-54bfbee877cf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControlHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""64d25ec9-6db7-4229-a62c-5fa46abc1ebd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControlHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6c0959f5-db1d-43a0-a5aa-b30efc4324f1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControlHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0f950ef5-50c8-43a7-b083-f47b903f20b3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControlHandleAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""PlayerActionMap"",
            ""id"": ""12c47ecc-3499-40fa-8d7e-2d5b316ac0f0"",
            ""actions"": [
                {
                    ""name"": ""HorizontalLookAction"",
                    ""type"": ""Value"",
                    ""id"": ""eb4cf211-743d-496c-acd5-d62dc1c5b821"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""VerticalLookAction"",
                    ""type"": ""Value"",
                    ""id"": ""e5cbd2d0-3928-4b0a-ab26-526b85809dec"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LookAction"",
                    ""type"": ""PassThrough"",
                    ""id"": ""68b6c913-ef72-40d8-93f6-ae40af0ec15d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""498c0f11-892d-469c-b6b4-0255a5f0a786"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""HorizontalLookAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e8d6013-be92-4dd6-91c3-7ceb51658085"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MnK Control Schema"",
                    ""action"": ""VerticalLookAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a5fa5b1-6908-4ee9-a5a4-d3dbd33f0e2a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookAction"",
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
        m_SpaceshipActionMap_ThrottleHandleAction = m_SpaceshipActionMap.FindAction("ThrottleHandleAction", throwIfNotFound: true);
        m_SpaceshipActionMap_ControlHandleAction = m_SpaceshipActionMap.FindAction("ControlHandleAction", throwIfNotFound: true);
        // PlayerActionMap
        m_PlayerActionMap = asset.FindActionMap("PlayerActionMap", throwIfNotFound: true);
        m_PlayerActionMap_HorizontalLookAction = m_PlayerActionMap.FindAction("HorizontalLookAction", throwIfNotFound: true);
        m_PlayerActionMap_VerticalLookAction = m_PlayerActionMap.FindAction("VerticalLookAction", throwIfNotFound: true);
        m_PlayerActionMap_LookAction = m_PlayerActionMap.FindAction("LookAction", throwIfNotFound: true);
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
    private readonly InputAction m_SpaceshipActionMap_ThrottleHandleAction;
    private readonly InputAction m_SpaceshipActionMap_ControlHandleAction;
    public struct SpaceshipActionMapActions
    {
        private @GalacticKittenInputControls m_Wrapper;
        public SpaceshipActionMapActions(@GalacticKittenInputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ThrottleHandleAction => m_Wrapper.m_SpaceshipActionMap_ThrottleHandleAction;
        public InputAction @ControlHandleAction => m_Wrapper.m_SpaceshipActionMap_ControlHandleAction;
        public InputActionMap Get() { return m_Wrapper.m_SpaceshipActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SpaceshipActionMapActions set) { return set.Get(); }
        public void AddCallbacks(ISpaceshipActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SpaceshipActionMapActionsCallbackInterfaces.Add(instance);
            @ThrottleHandleAction.started += instance.OnThrottleHandleAction;
            @ThrottleHandleAction.performed += instance.OnThrottleHandleAction;
            @ThrottleHandleAction.canceled += instance.OnThrottleHandleAction;
            @ControlHandleAction.started += instance.OnControlHandleAction;
            @ControlHandleAction.performed += instance.OnControlHandleAction;
            @ControlHandleAction.canceled += instance.OnControlHandleAction;
        }

        private void UnregisterCallbacks(ISpaceshipActionMapActions instance)
        {
            @ThrottleHandleAction.started -= instance.OnThrottleHandleAction;
            @ThrottleHandleAction.performed -= instance.OnThrottleHandleAction;
            @ThrottleHandleAction.canceled -= instance.OnThrottleHandleAction;
            @ControlHandleAction.started -= instance.OnControlHandleAction;
            @ControlHandleAction.performed -= instance.OnControlHandleAction;
            @ControlHandleAction.canceled -= instance.OnControlHandleAction;
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

    // PlayerActionMap
    private readonly InputActionMap m_PlayerActionMap;
    private List<IPlayerActionMapActions> m_PlayerActionMapActionsCallbackInterfaces = new List<IPlayerActionMapActions>();
    private readonly InputAction m_PlayerActionMap_HorizontalLookAction;
    private readonly InputAction m_PlayerActionMap_VerticalLookAction;
    private readonly InputAction m_PlayerActionMap_LookAction;
    public struct PlayerActionMapActions
    {
        private @GalacticKittenInputControls m_Wrapper;
        public PlayerActionMapActions(@GalacticKittenInputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @HorizontalLookAction => m_Wrapper.m_PlayerActionMap_HorizontalLookAction;
        public InputAction @VerticalLookAction => m_Wrapper.m_PlayerActionMap_VerticalLookAction;
        public InputAction @LookAction => m_Wrapper.m_PlayerActionMap_LookAction;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionMapActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Add(instance);
            @HorizontalLookAction.started += instance.OnHorizontalLookAction;
            @HorizontalLookAction.performed += instance.OnHorizontalLookAction;
            @HorizontalLookAction.canceled += instance.OnHorizontalLookAction;
            @VerticalLookAction.started += instance.OnVerticalLookAction;
            @VerticalLookAction.performed += instance.OnVerticalLookAction;
            @VerticalLookAction.canceled += instance.OnVerticalLookAction;
            @LookAction.started += instance.OnLookAction;
            @LookAction.performed += instance.OnLookAction;
            @LookAction.canceled += instance.OnLookAction;
        }

        private void UnregisterCallbacks(IPlayerActionMapActions instance)
        {
            @HorizontalLookAction.started -= instance.OnHorizontalLookAction;
            @HorizontalLookAction.performed -= instance.OnHorizontalLookAction;
            @HorizontalLookAction.canceled -= instance.OnHorizontalLookAction;
            @VerticalLookAction.started -= instance.OnVerticalLookAction;
            @VerticalLookAction.performed -= instance.OnVerticalLookAction;
            @VerticalLookAction.canceled -= instance.OnVerticalLookAction;
            @LookAction.started -= instance.OnLookAction;
            @LookAction.performed -= instance.OnLookAction;
            @LookAction.canceled -= instance.OnLookAction;
        }

        public void RemoveCallbacks(IPlayerActionMapActions instance)
        {
            if (m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionMapActions @PlayerActionMap => new PlayerActionMapActions(this);
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
        void OnThrottleHandleAction(InputAction.CallbackContext context);
        void OnControlHandleAction(InputAction.CallbackContext context);
    }
    public interface IPlayerActionMapActions
    {
        void OnHorizontalLookAction(InputAction.CallbackContext context);
        void OnVerticalLookAction(InputAction.CallbackContext context);
        void OnLookAction(InputAction.CallbackContext context);
    }
}
