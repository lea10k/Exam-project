using UnityEngine;

public interface IJumpBehaviour
{
    // Called when the player presses the jump button.
    void Jump();

    // Reset any internal state (e.g. after landing).
    void Reset();
}
