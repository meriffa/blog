namespace ByteZoo.Blog.Common.Models.Meals;

/// <summary>
/// Breakfast
/// </summary>
public class Breakfast : IDisposable, IAsyncDisposable
{

    #region Private Members
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// Breakfast drinks
    /// </summary>
    public List<Drink>? Drinks { get; set; }

    /// <summary>
    /// Breakfast food
    /// </summary>
    public List<Food>? Food { get; set; }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    ~Breakfast() => Dispose(false);

    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalization
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Free managed objects
                Drinks?.Clear();
                Drinks = null;
                Food?.Clear();
                Food = null;
            }
            // Free unmanaged resources
            disposed = true;
        }
    }

    /// <summary>
    /// Dispose managed resources
    /// </summary>
    /// <returns></returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        Drinks?.Clear();
        Drinks = null;
        Food?.Clear();
        Food = null;
        await ValueTask.CompletedTask;
    }
    #endregion

}