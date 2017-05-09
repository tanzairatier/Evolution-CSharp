using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Evolution
{
    class Animal
   {
        public int dir;

        public List<Point> direction_vectors = new List<Point>() { new Point(0,-1), new Point(0,1),
                                                                    new Point(1,0), new Point(-1,0),
                                                                    new Point(1,-1), new Point(1,1),
                                                                    new Point(-1,1), new Point(-1,-1),
                                                                    new Point(1,-2), new Point(2,-1),
                                                                    new Point(2,1), new Point(1,2),
                                                                    new Point(-1,2), new Point(-2,1),
                                                                    new Point(-2,-1), new Point(-1,-2),
                                                                    };
        public int energy;
        public List<int> genes;
        public int num_genes;
        public int x, y;
        private double mutation_rate = 0.80;
        List<int> new_genes;

        public Animal(int i, int j, int _animal_energy, int _num_genes, Random r)
        {
            x = i;
            y = j;
            energy = _animal_energy;
            dir = r.Next(8);
            genes = new List<int>();
            for (int _ = 0; _ < _num_genes; _++)
            {
                genes.Add(r.Next(2));
            }
            num_genes = _num_genes;
        }

        public void act(int energy_usage, Random r)
        {
            Point face = new Point();
            int choice = r.Next(genes.Sum());
            int sumsofar = 0;
            for (int i = 0; i < genes.Count(); i++)
            {
                sumsofar += genes.ElementAt(i);
                if (sumsofar == choice)
                {
                    face = direction_vectors.ElementAt(i);
                }
            }
            x += face.X;
            y += face.Y;
            energy = Math.Max(0, energy - energy_usage);
        }

        public void mutate(Random r)
        {
            new_genes = new List<int>();
            for (int i = 0; i < genes.Count; i++)
            {
                if (r.NextDouble() < mutation_rate)
                {
                    if (genes.ElementAt(i) == 0) {
                        new_genes.Add(1);
                    }
                    else
                    {
                        new_genes.Add(0);
                    }
                }
                else
                {
                    new_genes.Add(genes.ElementAt(i));
                }
            }
            genes = new_genes;
        }
    }
}
