using System.Collections.Generic;
using System.Data.Common;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            },
        });
    }

    [Fact]
    public void ConstructorWithNullCollectionThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void ConstructorWithEmptyCollectionThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new List<Recipe>(){null!})) ;
    }

    [Fact]
    public void ConstructionWithRecipeIdLowerOrEqualZeroThrows()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new List<Recipe>()
        {
            new Recipe()
            {
                Id = 0,
                Title = "Recipe F"
            },
        }));
    }

    [Fact]
    public void ConstructionWithNullOrWhiteSpaceRecipeTitleThrows()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new List<Recipe>()
        {
            new Recipe()
            {
                Id = 5,
                Title = "   "
            },
        }));
    }

    [Fact]
    public void ConstructionWithDuplicatedRecipeIdThrows()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new List<Recipe>()
        {
            new Recipe()
            {
                Id = 5,
                Title = "Recipe H"
            },

            new Recipe()
            {
                Id = 5,
                Title = "Recipe K"
            }
        }));
    }

    [Fact]
    public void AddRecipeWithNullRecipe()
    {
        var manager = CreateManager();
        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    public void AddRecipeWithRecipeIdLowerOrEqualZeroThrows()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(new Recipe()
        {
            Id = 0,
            Title = "Recipe C"
        }));
    }

    [Fact]
    public void AddRecipeWithNullOrWhiteSpaceRecipeTitleThrows()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(new Recipe()
        {
            Id = 5,
            Title = "  "
        }));
    }

    [Fact]
    public void AddRecipeWithDuplicatedRecipeIdThrows()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(new Recipe()
        {
            Id = 10,
            Title = "Recipe O"
        }));
    }

    [Fact]
    public void AddRecipeWithValidValue()
    {
        var manager = CreateManager();
        Assert.True(manager.AddRecipe(new Recipe()
        {
            Id = 6,
            Title = "Recipe U"
        }));
    }
}
