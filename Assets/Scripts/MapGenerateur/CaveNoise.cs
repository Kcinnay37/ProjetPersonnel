using UnityEngine;

public class CaveNoise
{
    // Attributs qui vont influencer le rendu de la caverne
    private float scale;
    private int octaves;
    private float persistence;
    private float lacunarity;
    private int seed;

    // Grille qui va contenir les valeurs de la caverne (0 pour la terre et 1 pour le vide)
    private int[,] caveGrid;

    // Constructeur qui prend en entr�e la largeur et la hauteur de la grille
    public CaveNoise(int width, int height, float scale, int octaves, float persistence, float lacunarity, int seed)
    {
        caveGrid = new int[width, height];
        this.scale = scale;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.seed = seed;
    }

    // Fonction qui g�n�re la caverne proc�duralement en utilisant l'algorithme de bruit de Perlin
    public void GenerateCave()
    {
        // On utilise un germe al�atoire pour initialiser la graine du g�n�rateur de nombres al�atoires
        System.Random rand = new System.Random(seed);
        // On utilise l'algorithme de bruit de Perlin pour g�n�rer un bruit de Perlin 2D
        float[,] noiseMap = Noise.GenerateNoiseMap(caveGrid.GetLength(0), caveGrid.GetLength(1), scale, octaves, persistence, lacunarity, rand.Next());

        // On parcourt tous les �l�ments de la grille
        for (int x = 0; x < caveGrid.GetLength(0); x++)
        {
            for (int y = 0; y < caveGrid.GetLength(1); y++)
            {
                // Si la valeur du bruit de Perlin est inf�rieure � 0,5, alors cet �l�ment de la grille sera de la terre (0)
                // Sinon, cet �l�ment sera du vide (1)
                caveGrid[x, y] = (noiseMap[x, y] < 0.5f) ? 0 : 1;
            }
        }
    }

    public void FillBorders()
    {
        int width = caveGrid.GetLength(0);
        int height = caveGrid.GetLength(1);

        // Remplissage de la premi�re et de la derni�re colonne
        for (int y = 0; y < height; y++)
        {
            caveGrid[0, y] = 1;
            caveGrid[width - 1, y] = 1;
        }

        // Remplissage de la premi�re et de la derni�re ligne
        for (int x = 0; x < width; x++)
        {
            caveGrid[x, 0] = 1;
            caveGrid[x, height - 1] = 1;
        }
    }

}
