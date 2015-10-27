# package-controller

## usage

Create a game object controller:

      - player input

Assign **ControllerKeyBinding**, etc. to the player input object.

A **ControllerBindingControl** will automatically be assigned to it; select
the object to attach handler at runtime to (itself is fine), the context
to deliver with events, and the fully qualified namespace of the event type
(see below).

Notice that the input manager may not be the game object for the player if
it is a prefab, as you cannot modify the component set on a prefab.

Then create a new custom script to bind to the event handler added to the
the input manager. For example, if 'input' is the input manager you might
use:

    using UnityEngine;
    using N.Package.Controller;
    using N;

    namespace Flyer.Airship {

      public enum PlayerInputType { // <--- This is the event type.
        TURN_LEFT,
        TURN_RIGHT,
        ACCELERATE,
        DECCELERATE,
        PITCH_UP,
        PITCH_DOWN,
        FLOAT_UP,
        FLOAT_DOWN,
        STOP_PITCH,
        STOP_FLOAT,
        STOP_TURNING,
        STOP_MOVING
      }

      /// Handle input and bind it to the movement state
      [AddComponentMenu("Flyer/Airship Input")]
      public class PlayerInput : ObjectControllerBase<PlayerInputType> {

        /// The controller to pull events from
        public void Start() {
          BindKeyDown(PlayerInputType.ACCELERATE, delegate(N.Input.Event e) {
            Console.Log("Found key press");
          });
        }
      }
    }

You will get an error like "You must set the controller on this object..." if you
do not assign the instance of the controller to the input handler.

Just to summarize the situation, you need:

- PlayerController (game object)
 - ControllerBindingControl
  - Input manager: The object to attach objects at runtime
  - Context: Reference to SomeObjectPrefabInstance
  - Qualified Name: Flyer.Airship.PlayerInputType

- SomeObjectPrefabInstance (game object)
 - PlayerInput
  - Controller: Reference to PlayerController

## Deferred binding

Notice that event handlers are bound on load by default, for 'up and go'
readiness.

However, if you spawn an object and want to bind input handlers to it,
use the 'deferBinding' flag on the controller control, and then manually
invoke BindHandlers():

    private void SetupInput(GameObject target) {
      if (inputController == null) {
        N.Console.Error("No input controller bound to landscape controller");
        return;
      }

      var binding = inputController.GetComponent<ControllerBindingControl>();
      binding.context = target;

      var instance = target.AddComponent<AirshipController>();
      instance.controller = inputController;

      // Bind events now everything is setup
      binding.BindHandlers();
    }
