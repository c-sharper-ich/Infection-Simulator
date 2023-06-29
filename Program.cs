using System.ComponentModel;
using System.Security.Cryptography;

static class Program
{
    static State[,] state;
    static int[,] day;
    static Random random;

    static async Task<int> Main()
    {
        int seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        random = new Random(seed);
        Console.WriteLine("感染シミュレーター");

        try
        {
            Console.Write("グリッド横\tW\t");
            WIDTH = Convert.ToInt32(Console.ReadLine());
            Console.Write("グリッド縦\tH\t");
            HEIGHT = Convert.ToInt32(Console.ReadLine());
            Console.Write("初期感染者数\tn_0\t");
            N_0 = Convert.ToInt32(Console.ReadLine());

            Task inittask = Init();

            Console.Write("潜伏期間\tD_1\t");
            D_1 = Convert.ToInt32(Console.ReadLine());
            Console.Write("発症期間\tD_2\t");
            D_2 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("確立に関しては100%を10000として入力してください");
            Console.Write("発症率\t\tP\t");
            P = int.MaxValue/10000* Convert.ToInt32(Console.ReadLine());

            Console.Write("感染率潜伏期間\tp_1\t");
            P_1 = int.MaxValue / 10000 * Convert.ToInt32(Console.ReadLine());
            Console.Write("感染率発症期間\tp_2\t");
            P_2 = int.MaxValue / 10000 * Convert.ToInt32(Console.ReadLine());
            Console.Write("感染者死亡率\tP_d\t");
            P_D = int.MaxValue / 10000 * Convert.ToInt32(Console.ReadLine());
            Console.Write("世界会議日数\tD\t");
            D = Convert.ToInt32(Console.ReadLine());
            Console.Write("隔離する (0:Yes)\t:");
            Isolation = 0==Convert.ToInt32(Console.ReadLine());

            List<string> texts = new List<string>();
            await inittask;
            Console.WriteLine("日\t\t無感染\t\t潜伏\t\t発症\t\t治癒\t死亡\t\t");
            texts.Add("日\t無感染\t潜伏\t発症\t治癒\t死亡\t");
            for (int day = 0; day < D; day++)
            {
                List<Task> tasks = new List<Task>();
                State[,] state_after = (State[,])state.Clone();
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        if (state_after[i,j] == State.Incubation)
                        {
                            tasks.Add(Among_Incubation(i, j));
                        }
                        if (state_after[i, j] == State.Illness)
                        {
                            tasks.Add(Among_Illness(i, j));
                        }
                    }
                }
                await Task.WhenAll(tasks);

                int[] counts = new int[6];

                for(int i=0;i<WIDTH;i++)
                {
                    for(int j=0;j<HEIGHT; j++) 
                    {
                        counts[(int)state[i, j]]++;
                    }
                }
                Console.WriteLine(day + "\t\t" + counts[0] + "\t\t" + counts[1] + "\t\t" + counts[2] + "\t\t" + counts[3] + "\t\t" + counts[4]);
                texts.Add(day + "\t" + counts[0] + "\t" + counts[1] + "\t" + counts[2] + "\t" + counts[3] + "\t" + counts[4]);
            }
            using (StreamWriter sw=new StreamWriter(AppContext.BaseDirectory+"result."+DateTime.Now.Month+"."+DateTime.Now.Day+"."+DateTime.Now.Hour+"."+DateTime.Now.Minute+"."+DateTime.Now.Second+".txt")) {
                for(int i=0;i< texts.Count; i++)
                {
                    await sw.WriteLineAsync(texts[i]);
                }
            }

        }
        catch (FormatException)
        {
            Console.WriteLine("不正な入力がされました");
            return -1;
        }
        return 0;
    }

    static Task Init()
    {
        state = new State[WIDTH, HEIGHT];
        day = new int[WIDTH, HEIGHT];
        for (int i = 0; i < N_0; i++)
        {
            int x, y;
            do
            {
                x = random.Next(WIDTH);
                y = random.Next(HEIGHT);
            } while (state[x, y] == State.Incubation);
            state[x, y] = State.Incubation;
        }

        return Task.CompletedTask;
    }

    static Task Among_Incubation(int x, int y)
    {
        if (random.Next() < P_1 && x - 1 >= 0 && state[x - 1, y] == State.Non_Infected)
        {
            state[x - 1, y] = State.Incubation;
            day[x - 1, y] = 0;
        }
        if (random.Next() < P_1 && x + 1 < WIDTH && state[x + 1, y] == State.Non_Infected)
        {
            state[x + 1, y] = State.Incubation;
            day[x + 1, y] = 0;
        }
        if (random.Next() < P_1 && y - 1 >= 0 && state[x, y - 1] == State.Non_Infected)
        {
            state[x, y - 1] = State.Incubation;
            day[x, y - 1] = 0;
        }
        if (random.Next() < P_1 && y + 1 < HEIGHT && state[x, y + 1] == State.Non_Infected)
        {
            state[x, y + 1] = State.Incubation;
            day[x, y + 1] = 0;
        }
        if (random.Next() < P_1 && x - 1 >= 0&&y-1>=0 && state[x - 1, y-1] == State.Non_Infected)
        {
            state[x - 1, y-1] = State.Incubation;
            day[x - 1, y-1] = 0;
        }
        if (random.Next() < P_1 && x + 1 < WIDTH &&y+1<HEIGHT&& state[x + 1, y+1] == State.Non_Infected)
        {
            state[x + 1, y+1] = State.Incubation;
            day[x + 1, y+1] = 0;
        }
        if (random.Next() < P_1 && y - 1 >= 0&&x+1<WIDTH && state[x+1, y - 1] == State.Non_Infected)
        {
            state[x+1, y - 1] = State.Incubation;
            day[x+1, y - 1] = 0;
        }
        if (random.Next() < P_1 && y + 1 < HEIGHT&&x-1>=0 && state[x-1, y + 1] == State.Non_Infected)
        {
            state[x-1, y + 1] = State.Incubation;
            day[x-1, y + 1] = 0;
        }
        day[x, y]++;
        if (state[x, y] == State.Incubation)
        {
            if (day[x, y] > D_1)
            {
                if (random.Next() < P)
                {
                    day[x, y] = 0;
                    state[x, y] = State.Illness;

                }
                else
                {
                    day[x, y] = 0;
                    state[x, y] = State.Healed;
                }
            }
        }

        return Task.CompletedTask;
    }

    static Task Among_Illness(int x, int y)
    {
        if (!Isolation)
        {
            if (random.Next() < P_2 && x - 1 >= 0 && state[x - 1, y] == State.Non_Infected)
            {
                state[x - 1, y] = State.Incubation;
                day[x - 1, y] = 0;
            }
            if (random.Next() < P_2 && x + 1 < WIDTH && state[x + 1, y] == State.Non_Infected)
            {
                state[x + 1, y] = State.Incubation;
                day[x + 1, y] = 0;
            }
            if (random.Next() < P_2 && y - 1 >= 0 && state[x, y - 1] == State.Non_Infected)
            {
                state[x, y - 1] = State.Incubation;
                day[x, y - 1] = 0;
            }
            if (random.Next() < P_2 && y + 1 < HEIGHT && state[x, y + 1] == State.Non_Infected)
            {
                state[x, y + 1] = State.Incubation;
                day[x, y + 1] = 0;
            }
            if (random.Next() < P_2 && x - 1 >= 0 && y - 1 >= 0 && state[x - 1, y - 1] == State.Non_Infected)
            {
                state[x - 1, y - 1] = State.Incubation;
                day[x - 1, y - 1] = 0;
            }
            if (random.Next() < P_2 && x + 1 < WIDTH && y + 1 < HEIGHT && state[x + 1, y + 1] == State.Non_Infected)
            {
                state[x + 1, y + 1] = State.Incubation;
                day[x + 1, y + 1] = 0;
            }
            if (random.Next() < P_2 && y - 1 >= 0 && x + 1 < WIDTH && state[x + 1, y - 1] == State.Non_Infected)
            {
                state[x + 1, y - 1] = State.Incubation;
                day[x + 1, y - 1] = 0;
            }
            if (random.Next() < P_2 && y + 1 < HEIGHT && x - 1 >= 0 && state[x - 1, y + 1] == State.Non_Infected)
            {
                state[x - 1, y + 1] = State.Incubation;
                day[x - 1, y + 1] = 0;
            }
        }
        if (random.Next() < P_D)
        {
            state[x, y] = State.Died;
            day[x, y] = 0;
        }
        else
        {
            day[x, y]++;
            if (day[x, y] > D_2)
            {
                day[x, y] = 0;
                state[x, y] = State.Healed;
            }
        }


        return Task.CompletedTask;
    }

    static int WIDTH = 1000, HEIGHT = 1000;
    static int N_0 = 10;
    static int P_1 = int.MaxValue / 10, P_2 = int.MinValue / 10, P_D = int.MaxValue / 1000, P = int.MaxValue / 3;
    static int D_1 = 10, D_2 = 7;
    static int D = 100;
    static bool Isolation=false;

}
enum State
{
    Non_Infected = 0, Incubation = 1, Illness = 2,  Healed = 3, Died = 4
}