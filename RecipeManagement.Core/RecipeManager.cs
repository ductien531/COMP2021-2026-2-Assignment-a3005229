using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    private Dictionary<int, Recipe> _recipes = new Dictionary<int, Recipe>();
    private LinkedList<int> _cookingPlan = new LinkedList<int>();

    private List<string> _shoppingList = new List<string>();
    private Stack<int> _removedRecipes = new Stack<int>();

    private Queue<string> _instructionsQueue = new Queue<string>();
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
        if (recipes is null)
        {
            throw new ArgumentNullException("Recipes can not be null");
        }

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentNullException("Recipe entry must not be null");
            }

            if (recipe.Id <= 0)
            {
                throw new ArgumentException("Recipe's ID must be positive");
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Recipe's title must not be null");
            }

            if (_recipes.ContainsKey(recipe.Id))
            {
                throw new ArgumentException("Recipe's must not be duplicated");
            }
            _recipes.Add(recipe.Id, recipe);
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionsQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException("Recipe entry must not be null");
        }

        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title) || _recipes.ContainsKey(recipe.Id))
        {
            return false;
        }

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return recipe;
        }
        else
        {
            return null;
        }
    }

    public bool RemoveRecipe(int recipeId)
    {
        if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }
        else
        {
            _recipes.Remove(recipeId);
            return true;
        }
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return 0;
        }
        else
        {
            _shoppingList.AddRange(recipe.Ingredients);
            return recipe.Ingredients.Count;
        }
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return new List<string>(_shoppingList);
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if(!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }
        else
        {
            _cookingPlan.AddLast(recipeId);
            return true;
        }
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        bool flag = _cookingPlan.Remove(recipeId);
        if (!flag)
        {
            return false;
        }
        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if(!_removedRecipes.TryPop(out int recipeID))
        {
            return false;
        }
        else
        {
            if(!_recipes.ContainsKey(recipeID) || _cookingPlan.Contains(recipeID))
            {
                return false;
            }
            else
            {
                _cookingPlan.AddLast(recipeID);
                return true;
            }
        }
    }

    public int? PeekLastRemovedRecipe()
    {
        if(_removedRecipes.TryPeek(out int recipeId))
        {
            return recipeId;
        }
        else
        {
            return null;
        }
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(_cookingPlan);
    }

    public bool StartCooking(int recipeId)
    {
        if(!_recipes.TryGetValue(recipeId, out Recipe? recipe) || recipe.Instructions.Count == 0)
        {
            return false;
        }
        else
        {
            _instructionsQueue.Clear();
            foreach(string value in recipe.Instructions)
            {
                _instructionsQueue.Enqueue(value);
            }
            
            return true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string? PeekNextInstruction()
    {
        if(_instructionsQueue.TryPeek(out string? instruction))
        {
            return instruction;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string? CompleteNextInstruction()
    {
        if(_instructionsQueue.TryDequeue(out string? instruction))
        {
            return instruction;
        }
        else
        {
            return null;
        }
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
