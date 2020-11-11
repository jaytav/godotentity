using Godot;
using Godot.Collections;

public class StateMachine : Node {
    [Signal]
    public delegate void Transitioned(State toState);

    [Export]
    private NodePath initialState;

    private State state;

    public async override void _Ready() {
        AddToGroup("StateMachine");
        await ToSignal(Owner, "ready");

        if (initialState == null) {
            GD.PushError("StateMachine.initialState not initialized");
        }

        state = GetNode<State>(initialState);
        state.Enter();
        EmitSignal("Transitioned", state);
    }

    public override void _Process(float delta) {
        state.Process(delta);
    }

    public override void _PhysicsProcess(float delta) {
        state.PhysicsProcess(delta);
    }

    public override void _UnhandledInput(InputEvent @event) {
        state.UnhandledInput(@event);
    }

    public void TransitionTo(string targetStatePath, Dictionary msg = null) {
        if (!HasNode(targetStatePath)) {
            return;
        }

        state.Exit();
        state = GetNode<State>(targetStatePath);
        state.Enter(msg);
        EmitSignal("Transitioned", state);
    }

    private void _on_StateMachine_Transitioned(State toState) {
        GD.Print(Owner.Name + " transitioned to " + toState.Name);
    }
}
