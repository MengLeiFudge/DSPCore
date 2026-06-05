# Recipes

## Responsibility

This block declares custom recipe types and the machines allowed to use them.

## Public API

- `RecipeTypeDescriptor`
- `RecipeTypeRegistry`

## Example

- `Examples/RecipeTypeExample.cs`

## Runtime

`Runtime/RecipeTypeRuntime.cs` marks declared recipes as custom and blocks unsupported assembler recipe selection.

## Boundaries

This block does not create recipes. Recipe creation belongs to proto registration; this block classifies and guards existing recipe ids.
