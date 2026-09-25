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

    /// <summary>
    /// This method will build the recipe catalogue from the supplied recipes
    /// </summary>
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
        if (recipes is null) // If supplied recipes is null, it will throw ArgumentNullException
        {
            throw new ArgumentNullException("Recipes can not be null");
        }

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentNullException("Recipe entry must not be null");
            }

            if (recipe.Id <= 0) // Recipe ID can not be negative or equal 0
            {
                throw new ArgumentException("Recipe's ID must be positive");
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Recipe's title must not be null");
            }

            if (_recipes.ContainsKey(recipe.Id)) // Dictionary can not have duplicated keys
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

    /// <summary>
    /// This method will add supplied recipe into the dictionary (_recipes). Besides that, invalid value will be rejected and 
    /// return False
    /// </summary>
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

        _recipes.Add(recipe.Id, recipe); // This valid value will be added to the dictionary
        return true;
    }

    public Recipe FindRecipe(int recipeId)
    {
        if (_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return recipe;
        }
        else
        {
            return null!;
        }
    }

    /// <summary>
    /// This method will remove a recipe from the dictionary. If the recipe Id exists in the _cookingPlan, it can not be 
    /// deleted and return false
    /// </summary>
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

    /// <summary>
    /// This method will copy a recipe from the original _recipes and add recipe's ingredient into _shoppingList. It will 
    /// return 0 if the recipe is unfounded
    /// </summary>
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

    /// <summary>
    /// This method will remove a recipe from _cookingPlan and push it to _removedRecipes as the most recently removed
    /// </summary>
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

    /// <summary>
    /// This method will return the most recently removed recipe and place it to the last index of the cooking plan
    /// </summary>
    public bool RestoreLastRemovedRecipe()
    {
        if(!_removedRecipes.TryPop(out int recipeID)) // It will return false if the recipe Id doesn't exist in _removedRecipes
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

    /// <summary>
    /// For an existing recipe with at least one instruction, clear the current queue, enqueue the instruction strings in their original order and return true. Otherwise return false
    /// </summary>
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
    /// This method will peek the nex instruction from the _instructionsQueue. If the _instructionQueue doesn't have any instruction, it will return null
    /// </summary>
    /// <returns></returns>
    public string PeekNextInstruction()
    {
        if(_instructionsQueue.TryPeek(out string? instruction))
        {
            return instruction;
        }
        else
        {
            return null!;
        }
    }

    /// <summary>
    /// This method will  removes and returns exactly one item from the front
    /// </summary>
    public string CompleteNextInstruction()
    {
        if(_instructionsQueue.TryDequeue(out string? instruction))
        {
            return instruction;
        }
        else
        {
            return null!;
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
