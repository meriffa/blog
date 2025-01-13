using ByteZoo.Blog.Common.Models.People;

namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee
/// </summary>
/// <param name="position"></param>
[EmployeeSchema("1.0.0")]
public class Employee(string position) : Person
{

    #region Properties
    /// <summary>
    /// Employee events
    /// </summary>
    public string Position { get; private set; } = position;

    /// <summary>
    /// Employee base salary
    /// </summary>
    public required decimal BaseSalary { get; set; }

    /// <summary>
    /// Employee events
    /// </summary>
    public required List<EmployeeEvent> Events { get; set; }
    #endregion

    #region Events
    /// <summary>
    /// Position changed event
    /// </summary>
    public event EventHandler<PositionChangedArgs>? PositionChanged;
    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPosition"></param>
    public void ChangePosition(string currentPosition)
    {
        var originalPosition = Position;
        Position = currentPosition;
        OnPositionChanged(new PositionChangedArgs(originalPosition, currentPosition));
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Raise position changed event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnPositionChanged(PositionChangedArgs e) => PositionChanged?.Invoke(this, e);
    #endregion

}