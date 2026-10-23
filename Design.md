# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

Name the C# collection you selected for the saved/favorite feature and explain why it suits add, remove, duplicate prevention and membership checks.

I chose HashSet for the saved feature. The assignment requires the _savedRecipe contains no duplicated recipe Id and have no order, so this data structure matches these requirements.

All three of those operations (add, remove, duplicate) are average O(1), compared to List O(n) and other collections.


## 2. Integration

In a short paragraph, explain how the new Part B features use the existing Part A `RecipeManager` and recipe catalogue.

The part B features read from the same Dictionary<int, Recipe> catalogue that part A builds in the constructor. Both part A and part B use an original version of the recipe data. All methods in part A such as AddRecipe(), AddRecipeToCookingPlan(), AddIngredientsToShoppingList(), etc change the data structures in the RecipeManager, So when I test an integrated workflow, all data is related, creating a complete workflow. All features in part B use the Recipes Dictionary to search for a recipe by title or ingredients through the passed parameter 

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | HashSet | Average O(1) | HashSet is used because it prevents duplicate recipe IDs and checks quickly whether a recipe is saved. Checking takes O(1) time on average, but can take O(n) if many IDs fall into the same bucket. |
