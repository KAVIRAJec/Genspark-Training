export class RecipeModel {
    public id: number;
    public name: string;
    public ingredients: string[];
    prepTimeMinutes: number;
    public cuisine: string;
    public difficulty: string;

    constructor(id: number, name: string, ingredients: string[], prepTimeMinutes: number, cuisine: string, difficulty: string) {
        this.id = id;
        this.name = name;
        this.ingredients = ingredients;
        this.prepTimeMinutes = prepTimeMinutes;
        this.cuisine = cuisine;
        this.difficulty = difficulty;
    }
}