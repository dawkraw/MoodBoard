using ValueOf;

namespace Domain.ValueObjects;

public class CanvasRotation : ValueOf<float, CanvasRotation>
{
    private const float MinimumDegrees = -360f;
    private const float MaximumDegrees = 360f;

    protected override void Validate()
    {
        if (Value is > MaximumDegrees or < MinimumDegrees)
            throw new ArgumentException("Degrees have to be between 0 and 360.");
    }
}