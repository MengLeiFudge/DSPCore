# Recipes

## Responsibility

This block declares custom recipe types and the machines allowed to use them.

## Public API

- `RecipeTypes`: author-facing short entry point.
- `RecipeTypeDescriptor`
- `RecipeTypeRegistry`

## Example

- `Examples/RecipeTypeExample.cs`

## Runtime

`RecipeTypeRuntime.cs` marks declared recipes as custom and blocks unsupported assembler recipe selection.

## Boundaries

This block does not create recipes. Recipe creation belongs to proto registration; this block classifies and guards existing recipe ids.
