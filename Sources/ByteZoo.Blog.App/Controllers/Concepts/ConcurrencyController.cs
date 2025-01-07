using ByteZoo.Blog.Common.Models.Concurrency;
using CommandLine;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Concurrency controller
/// </summary>
[Verb("Concepts-Concurrency", HelpText = "Concurrency operation.")]
public class ConcurrencyController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute() => Task.Run(async () =>
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        await using (var breakfast = await PrepareBreakfastAsync(2, 3, 4, cancellationTokenSource.Token))
            displayService.WriteInformation($"[Breakfast] Drinks = {breakfast.Drinks?.Count}, Food = {breakfast.Food?.Count}.");
        displayService.Wait();
    }).Wait();
    #endregion

    #region Private Methods
    /// <summary>
    /// Prepare breakfast
    /// </summary>
    /// <param name="eggCount"></param>
    /// <param name="baconSlices"></param>
    /// <param name="breadSlices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Breakfast> PrepareBreakfastAsync(int eggCount, int baconSlices, int breadSlices, CancellationToken cancellationToken)
    {
        var breakfast = new Breakfast() { Drinks = [], Food = [] };
        try
        {
            displayService.WriteInformation("[Breakfast] Started ...");
            var coffee = await PourCoffeeAsync();
            breakfast.Drinks.Add(coffee);
            cancellationToken.ThrowIfCancellationRequested();
            var eggsTask = FryEggsAsync(eggCount, cancellationToken);
            var baconTask = FryBaconAsync(baconSlices, cancellationToken);
            var toastTask = MakeToastWithButterAndJamAsync(breadSlices, cancellationToken);
            var foodItems = await Task.WhenAll(eggsTask, baconTask, toastTask);
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var foodItem in foodItems)
                breakfast.Food.AddRange(foodItem);
            var orangeJuice = await PourOrangeJuiceAsync();
            breakfast.Drinks.Add(orangeJuice);
            await foreach (var item in ServeFoodAsync(breakfast.Food, cancellationToken))
                displayService.WriteInformation($"[Serving] {item.GetType().Name} ...");
            displayService.WriteInformation("[Breakfast] Ready.");
        }
        catch (Exception ex)
        {
            displayService.WriteError(ex);
        }
        return breakfast;
    }

    /// <summary>
    /// Simulate activity (IO-bound)
    /// </summary>
    /// <param name="secondsDelay"></param>
    private static Task SimulateActivityIO(int secondsDelay) => Task.Delay(secondsDelay * Random.Shared.Next(800, 1201));

    /// <summary>
    /// Simulate activity (CPU-bound)
    /// </summary>
    /// <param name="secondsDelay"></param>
    /// <returns></returns>
    private static Task SimulateActivityCPU(int secondsDelay) => Task.Run(() =>
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var duration = secondsDelay * Random.Shared.Next(800, 1201);
        var i = 0;
        while (stopwatch.ElapsedMilliseconds < duration)
            i = (i + 1) % 1024;
        stopwatch.Stop();
    });

    /// <summary>
    /// Return noun ending
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private static string GetNounEnding(int count) => count == 1 ? string.Empty : "s";

    /// <summary>
    /// Pour coffee
    /// </summary>
    /// <returns></returns>
    private async ValueTask<Coffee> PourCoffeeAsync()
    {
        displayService.WriteInformation("[Coffee] Pouring ...");
        await SimulateActivityIO(1);
        displayService.WriteInformation("[Coffee] Ready.");
        return new();
    }

    /// <summary>
    /// Fry eggs
    /// </summary>
    /// <param name="eggCount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Food[]> FryEggsAsync(int eggCount, CancellationToken cancellationToken)
    {
        displayService.WriteInformation("[Eggs] Started ...");
        var eggs = new List<Egg>();
        for (int i = 0; i < eggCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            eggs.Add(new());
            displayService.WriteInformation($"[Eggs] Cracking egg #{i + 1} ...");
            await SimulateActivityIO(1);
            if (i + 1 >= 5)
                throw new("Too many eggs in the pan.");
        }
        displayService.WriteInformation($"[Eggs] Cooking the egg{GetNounEnding(eggCount)} ...");
        await SimulateActivityIO(3);
        displayService.WriteInformation("[Eggs] Ready.");
        return [.. eggs];
    }

    /// <summary>
    /// Fry bacon
    /// </summary>
    /// <param name="baconSlices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Food[]> FryBaconAsync(int baconSlices, CancellationToken cancellationToken)
    {
        displayService.WriteInformation($"[Bacon] Started ...");
        var bacon = new List<Bacon>();
        displayService.WriteInformation("[Bacon] Cooking first side of bacon ...");
        await SimulateActivityIO(3);
        for (int i = 0; i < baconSlices; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bacon.Add(new());
            displayService.WriteInformation($"[Bacon] Flipping slice of bacon #{i + 1} ...");
            await SimulateActivityIO(1);
        }
        displayService.WriteInformation("[Bacon] Cooking the second side of bacon ...");
        await SimulateActivityIO(3);
        displayService.WriteInformation("[Bacon] Ready.");
        return [.. bacon];
    }

    /// <summary>
    /// Make toast with butter and jam
    /// </summary>
    /// <param name="breadSlices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Food[]> MakeToastWithButterAndJamAsync(int breadSlices, CancellationToken cancellationToken)
    {
        displayService.WriteInformation("[Toast] Started.");
        var toast = await ToastBreadAsync(breadSlices, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ApplyButter(toast);
        ApplyJam(toast);
        displayService.WriteInformation("[Toast] Ready.");
        return toast;
    }

    /// <summary>
    /// Toast bread
    /// </summary>
    /// <param name="breadSlices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Toast[]> ToastBreadAsync(int breadSlices, CancellationToken cancellationToken)
    {
        var toast = new List<Toast>();
        for (int i = 0; i < breadSlices; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            toast.Add(new() { Spreads = [] });
            displayService.WriteInformation($"[Toast] Putting slice of bread #{i + 1} in the toaster ...");
            await SimulateActivityIO(1);
        }
        displayService.WriteInformation("[Toast] Toasting ...");
        await SimulateActivityIO(3);
        displayService.WriteInformation($"[Toast] Removing {breadSlices} slice{GetNounEnding(breadSlices)} of bread from the toaster ...");
        return [.. toast];
    }

    /// <summary>
    /// Apply butter
    /// </summary>
    /// <param name="toast"></param>
    private void ApplyButter(Toast[] toast)
    {
        for (int i = 0; i < toast.Length; i++)
        {
            toast[i].Spreads.Add(new Butter());
            displayService.WriteInformation($"[Butter] Putting butter on toast #{i + 1} ...");
        }
    }

    /// <summary>
    /// Apply jam
    /// </summary>
    /// <param name="toast"></param>
    private void ApplyJam(Toast[] toast)
    {
        for (int i = 0; i < toast.Length; i++)
        {
            toast[i].Spreads.Add(new Jam());
            displayService.WriteInformation($"[Jam] Putting jam on toast #{i + 1} ...");
        }
    }

    /// <summary>
    /// Pour orange juice
    /// </summary>
    /// <returns></returns>
    private async ValueTask<OrangeJuice> PourOrangeJuiceAsync()
    {
        displayService.WriteInformation("[Orange Juice] Pouring ...");
        await SimulateActivityCPU(1);
        displayService.WriteInformation("[Orange Juice] Ready.");
        return new();
    }

    /// <summary>
    /// Serve food
    /// </summary>
    /// <param name="foodItems"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async IAsyncEnumerable<Food> ServeFoodAsync(List<Food> foodItems, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var foodItem in foodItems)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await SimulateActivityIO(1);
            yield return foodItem;
        }
    }
    #endregion

}