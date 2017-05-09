using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Evolution
{
    public partial class Evolution : Form
    {
        private Pen lush_box_pen = new Pen(Color.Green, 3);
        private Point mouse_box = new Point();
        private Pen mouse_box_pen = new Pen(Color.Yellow, 3);
        private int mouse_square_size = 5;
        private int mouse_square_size_max = 45;
        private int mouse_square_size_min = 3;
        private bool play_mode = false;
        private double play_speed = 1.00;
        private bool ready_to_run = true;
        private bool draw_grid = false;
        private System.Timers.Timer timer;
        private World world = new World();
        
        public Evolution()
        {
            mouse_box_pen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
            InitializeComponent();
        }

        public void populate()
        {
            world.animals = new List<Animal>();
            world.initialize_plant_map();
            world.grow_plants(world.plant_growth_rate);
            world.grow_animals();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            populate();
            Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (play_mode && play_speed > 0.0001)
            {
                play_speed /= 2;
                timer.Interval = 1000 * play_speed;
                label1.Text = "Auto Speed Control:" + (int)1/play_speed + "x";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (play_mode && play_speed < 1)
            {
                play_speed *= 2;
                timer.Interval = 1000 * play_speed;
                label1.Text = "Auto Speed Control:" + (int)1 / play_speed + "x";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            play_mode = true;
            timer = new System.Timers.Timer(1000 * play_speed);
            timer.Enabled = true;
            timer.Elapsed += timer_Elapsed;
            button5.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String input = textBox1.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 1;
                textBox1.Text = "1";
            }
            for (int i = 0; i < value; i++)
            {
                world.do_one_day();
            }
            Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            world.wrap_world = checkBox1.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
               BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
               null, panel1, new object[] { true });
            textBox1.Text = "1";
            textBox2.Text = "" + world.plant_regrowth_rate;
            textBox3.Text = "" + world.plant_lush_regrowth_rate;
            textBox4.Text = "" + world.animal_energy_usage;
            textBox5.Text = "" + world.reprod_energy;
            textBox6.Text = "" + world.initial_animals;
            textBox7.Text = "" + world.plant_energy;
            world.num_cells_wide = (int)Math.Floor((double)panel1.Width / world.cell_width);
            world.num_cells_high = (int)Math.Floor((double)panel1.Height / world.cell_height);
            world.initialize_plant_map();
        }

        private void panel1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            world.lush_topleft = new Point(mouse_box.X, mouse_box.Y);
            world.lush_square_size = mouse_square_size;
            Refresh();
        }

        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouse_box = new Point(e.X / world.cell_width, e.Y / world.cell_height);
            Refresh();
        }
        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouse_square_size += e.Delta / 120;
            if (mouse_square_size < mouse_square_size_min)
            {
                mouse_square_size = mouse_square_size_min;
            }
            if (mouse_square_size > mouse_square_size_max)
            {
                mouse_square_size = mouse_square_size_max;
            }
            Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Graphics graphics = e.Graphics;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            //drawing the grid
            if (draw_grid)
            {
                for (int i = 0; i <= world.num_cells_wide; i++)
                {
                    graphics.DrawLine(Pens.Black, new Point(i * world.cell_width, 0), new Point(i * world.cell_width, world.num_cells_high * world.cell_height));
                }
                if (panel1.Width % world.cell_width == 0) //in case the last column line gets cut off because panel width divides cell width perfectly
                {
                    graphics.DrawLine(Pens.Black, new Point(panel1.Width - 1, 0), new Point(panel1.Width - 1, world.num_cells_high * world.cell_height));
                }
                for (int j = 0; j <= world.num_cells_high; j++)
                {
                    graphics.DrawLine(Pens.Black, new Point(0, j * world.cell_height), new Point(world.num_cells_wide * world.cell_width, j * world.cell_height));
                }
                if (panel1.Height % world.cell_height == 0) //in case the last row line gets cut off because panel height divides cell height perfectly
                {
                    graphics.DrawLine(Pens.Black, new Point(0, panel1.Height - 1), new Point(world.num_cells_wide * world.cell_width, panel1.Height - 1));
                }
            }

            try
            {
                foreach (Plant plant in world.plants)
                {
                    if (plant != null)
                    {
                         graphics.FillEllipse(Brushes.ForestGreen, plant.x * world.cell_width, plant.y * world.cell_height, world.cell_width, world.cell_height);
                    }
                }
            }
            catch {
                ;
            }
            try
            {
                foreach (Animal animal in world.animals)
                {
                    graphics.FillEllipse(Brushes.Orange, animal.x * world.cell_width, animal.y * world.cell_height, world.cell_width, world.cell_height);
                    //graphics.FillRectangle(Brushes.Red, animal.x * world.cell_width, animal.y * world.cell_height + 0.85f * world.cell_height, (animal.energy / 100.0f) * world.cell_width, 0.15f * world.cell_height);
                }
            }
            catch{
                ;
            }

            graphics.DrawRectangle(mouse_box_pen, mouse_box.X * world.cell_width, mouse_box.Y * world.cell_height, world.cell_width * mouse_square_size, world.cell_height * mouse_square_size);
            graphics.DrawRectangle(lush_box_pen, world.lush_topleft.X * world.cell_width, world.lush_topleft.Y * world.cell_height, world.cell_width * world.lush_square_size, world.cell_height * world.lush_square_size);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                world.animal_num_genes = 4;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                world.animal_num_genes = 8;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                world.animal_num_genes = 16;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox2.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox2.Text = "" + value;
            world.plant_regrowth_rate = value;
        }

        private void textBox3_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox3.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox3.Text = "" + value;
            world.plant_lush_regrowth_rate = value;
        }

        private void textBox4_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox4.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox4.Text = "" + value;
            world.animal_energy_usage = value;
        }

        private void textBox5_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox5.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox5.Text = "" + value;
            world.reprod_energy = value;
        }

        private void textBox6_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox6.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox6.Text = "" + value;
            world.initial_animals = value;
        }

        private void textBox7_LeaveEvent(object sender, EventArgs e)
        {
            String input = textBox7.Text;
            int value = 0;
            try
            {
                value = Convert.ToInt32(input);
            }
            catch (Exception)
            {
                value = 10;
            }
            textBox7.Text = "" + value;
            world.plant_energy = value;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ready_to_run)
            {
                ready_to_run = false;
                world.do_one_day();
                if (world.animals.Count == 0) {
                    populate();
                }
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        panel1.Refresh();
                    });
                }
                catch (Exception)
                {
                    ;//window closed
                }
                ready_to_run = true;
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            draw_grid = checkBox2.Checked;
            Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            play_mode = false;
            timer.Stop();
            timer.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }
    }
}