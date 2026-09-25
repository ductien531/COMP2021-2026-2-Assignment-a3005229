using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using Microsoft.VisualBasic;
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
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new List<Recipe>() { null! }));
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

    [Fact]
    public void FindRecipeWithValidValue()
    {
        var manager = CreateManager();
        var foundRecipe = manager.FindRecipe(10);
        Assert.Equal("Recipe A", foundRecipe!.Title);
    }

    [Fact]
    public void FindRecipeWithInvalidRecipeId()
    {
        var manager = CreateManager();
        var searchedRecipe = manager.FindRecipe(11);
        Assert.Null(searchedRecipe);
    }

    [Fact]
    public void RemoveRecipeWithRecipeId()
    {
        var manager = CreateManager();
        var removeRecipe = manager.RemoveRecipe(10);
        Assert.True(removeRecipe);
    }

    [Fact]
    public void RemoveRecipeWithExistingRecipeIdInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        Assert.False(manager.RemoveRecipe(10));
    }

    [Fact]
    public void AddIngredientsToShoppingListWithUnfoundedRecipeId()
    {
        var manager = CreateManager();
        var addedIngredients = manager.AddIngredientsToShoppingList(4);
        Assert.Equal(0, addedIngredients);
    }

    [Fact]
    public void AddIngredientsToShoppingListWithFoundedRecipeId()
    {
        var manager = CreateManager();
        var addedIngredients = manager.AddIngredientsToShoppingList(10);
        Assert.Equal(1, addedIngredients);
    }

    [Fact]
    public void GetShoppingList()
    {
        List<string> shoppingList = new();
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        shoppingList.Add("1 apple");
        Assert.Equal(shoppingList, manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingListWithTest()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddRecipeToCookingPlanWithInvalidValue()
    {
        var manager = CreateManager();
        bool addedRecipe = manager.AddRecipeToCookingPlan(2);
        Assert.False(addedRecipe);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlanWithInvalidRecipeId()
    {
        var manager = CreateManager();
        var removedRecipe = manager.RemoveRecipeFromCookingPlan(12);
        Assert.False(removedRecipe);
    }

    [Fact]
    public void RestoreLastRemovedRecipeWithEmptyRemovedRecipe()
    {
        var manager = CreateManager();
        var poppedRecipe = manager.RestoreLastRemovedRecipe();
        Assert.False(poppedRecipe);
    }

    [Fact]
    public void RestoreLastRemovedRecipeWithInvalidValue()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.AddRecipeToCookingPlan(10);
        var restoredRecipe = manager.RestoreLastRemovedRecipe();
        Assert.False(restoredRecipe);
    }

    [Fact]
    public void RestoreLastRemovedRecipeWithNonExistingRecipeId()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipe(10);
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void PeekLastRemovedRecipeWithEmptyRecipeIdInRemovedRecipe()
    {
        var manager = CreateManager();
        var peekItem = manager.PeekLastRemovedRecipe();
        Assert.Null(peekItem);
    }

    [Fact]
    public void StartCookingWithEmptyRecipeId()
    {
        var manager = CreateManager();
        manager.RemoveRecipe(10);
        var cooking = manager.StartCooking(10);
        Assert.False(cooking);
    }

    [Fact]
    public void PeekNextInstructionWithEmptyInstruction()
    {
        var manager = CreateManager();
        Assert.Null(manager.PeekNextInstruction());
    }

    [Fact]
    public void CompleteNextInstruction()
    {
        var manager = CreateManager();
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void CollectionsWithEmpty()
    {
        var manager = CreateManager();
        Assert.Equal(manager.CookingPlanCount, 0);
        Assert.Equal(manager.PendingInstructionCount, 0);
        Assert.Equal(manager.RemovedRecipeCount, 0);
    }
}
