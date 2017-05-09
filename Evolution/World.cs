using System;
using System.Collections.Generic;
using System.Drawing;

namespace Evolution
{
    class World
    {
        private Animal child;
        private Random randomizer = new Random();
        public bool wrap_world = true;
        public int animal_energy = 100;
        public int animal_energy_usage = 2;
        public int animal_num_genes = 8;
        public int cell_height = 8;
        public int cell_width = 8;
        public int initial_animals = 10;
        public int num_cells_wide, num_cells_high;
        public int plant_energy = 80;
        public int plant_growth_rate = 10;
        public int plant_lush_regrowth_rate = 5;
        public int plant_regrowth_rate = 10;
        public int reprod_energy = 150;
        public List<Animal> animals;
        public List<Animal> living_animals;
        public Plant[,] plants;

        public World()
        {
            plants = new Plant[num_cells_wide, num_cells_high];
            animals = new List<Animal>();
            lush_square_size = 9;
        }

        public int lush_square_size { get; internal set; }
        public Point lush_topleft { get; internal set; }
        public void do_one_day()
        {
            living_animals = new List<Animal>();
            foreach (Animal animal in animals)
            {
                animal.act(animal_energy_usage, randomizer);
                if (animal.x >= num_cells_wide)
                {
                    if (wrap_world)
                    {
                        animal.x = 0;
                    }
                    else
                    {
                        animal.x = num_cells_wide - 1;
                    }
                }
                if (animal.x < 0)
                {
                    if (wrap_world)
                    {
                        animal.x = num_cells_wide - 1;
                    }
                    else
                    {
                        animal.x = 0;
                    }
                }
                if (animal.y >= num_cells_high)
                {
                    if (wrap_world)
                    {
                        animal.y = 0;
                    }
                    else
                    {
                        animal.y = num_cells_high - 1;
                    }
                }
                if (animal.y < 0)
                {
                    if (wrap_world)
                    {
                        animal.y = num_cells_high - 1;
                    }
                    else
                    {
                        animal.y = 0;
                    }
                }
                if (plants[animal.x, animal.y] != null)
                {
                    animal.energy += plants[animal.x, animal.y].energy;
                    plants[animal.x, animal.y] = null;
                }
                if (animal.energy > 0)
                {
                    living_animals.Add(animal);
                }
                if (animal.energy > reprod_energy)
                {   //reproduce
                    animal.energy /= 2;
                    child = new Animal(animal.x, animal.y, animal.energy, animal.num_genes, randomizer);
                    child.genes = animal.genes;
                    child.mutate(randomizer);
                    living_animals.Add(child);
                }
            }

            animals = living_animals;

            //regrowth of plants in the world
            grow_plants(plant_regrowth_rate);
            grow_lush_plants(plant_lush_regrowth_rate);

        }

        public void grow_animals()
        {
            Random r = new Random();
            for (int i = 0; i < initial_animals; i++)
            {
                animals.Add(new Animal(r.Next(num_cells_wide), r.Next(num_cells_high), animal_energy, animal_num_genes, randomizer));
            }
        }

        public void grow_lush_plants(int rate)
        {
            for (int _ = 0; _ < rate; _++)
            {
                int i = randomizer.Next(Math.Max(0, lush_topleft.X), Math.Min(lush_topleft.X + lush_square_size, num_cells_wide));
                int j = randomizer.Next(Math.Max(0, lush_topleft.Y), Math.Min(lush_topleft.Y + lush_square_size, num_cells_high));
                plants[i, j] = new Plant(i, j, plant_energy);

            }
        }

        public void grow_plants(int rate)
        {
            for (int _ = 0; _ < rate; _++)
            {
                int i = randomizer.Next(0, num_cells_wide);
                int j = randomizer.Next(0, num_cells_high);
                plants[i, j] = new Plant(i, j, plant_energy);
            }
        }

        public void initialize_plant_map()
        {
            plants = new Plant[num_cells_wide, num_cells_high];
            for (int i = 0; i < num_cells_wide; i++)
            {
                for (int j = 0; j < num_cells_high; j++)
                {
                    plants[i, j] = null;
                }
            }
        }
    }
}
