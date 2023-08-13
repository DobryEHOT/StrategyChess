using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    public int ID { get; private set; } // Уникальный идентификатор. Нужен для дебага

    private int[] layers;//Слои
    private float[][] neurons;//Нейроны
    private float[][] biases;//Смещения
    private float[][][] weights;//Веса
    private int[] activations;//layers  Данный массив не используется.

    public float Fitness { get;  set; } = 0;//Показатель эффективности

    private System.Random rnd = new Random(); // Класс для генерации случайных величин 

    public NeuralNetwork(int[] layers, int id)
    {
        ID = id;

        //Копирование слоёв
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //Инициализация нейронов, смещений, весов
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    private void InitNeurons()//Создание пустого массива для хранения нейронов.
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitBiases()//Инициализирует и заполняет массив для смещений
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {


                bias[j] = rnd.Next(-5, 5) / 10f;
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitWeights()//Инициализирует массив весов со случайными весами
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //float sd = 1f / ((neurons[i].Length + neuronsInPreviousLayer) / 2f);
                    neuronWeights[k] = rnd.Next(-5, 5) / 10f;
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)//Прямая транстяция, входы >==> выходы.
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = activate(value + biases[i][j]);
            }
        }
        return neurons[neurons.Length - 1];
    }

    public float activate(float value) // Функция активации
    {

        return (float)Math.Tanh(value);
    }

    public void Mutate(int chance, float val)// Функция мутации для реализации генетического алгоритма
    {
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < biases.Length; i++)
        {
            int temp = i;
            tasks.Add(Task.Factory.StartNew(() => mutateLayerBeases(chance, val, temp)));
        }
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();

        for (int i = 0; i < weights.Length; i++)
        {
            int temp = i;

            tasks.Add(Task.Factory.StartNew(() => mutateLayersWeight(chance, val, temp)));
        }
        Task.WaitAll(tasks.ToArray());

    }

    private void mutateLayersWeight(int chance, float val, int i)
    {
        for (int j = 0; j < weights[i].Length; j++)
        {
            for (int k = 0; k < weights[i][j].Length; k++)
            {
                lock (rnd)
                    weights[i][j][k] = (Math.Round(rnd.NextDouble() * chance, 13) <= 5) ? weights[i][j][k] += (float)Math.Round(-val + rnd.NextDouble() * (val + val), 13) : weights[i][j][k];
            
            }
            Thread.Yield();
        }
    }

    private void mutateLayerBeases(int chance, float val, int i)
    {
        for (int j = 0; j < biases[i].Length; j++)
        {
            lock (rnd)
                biases[i][j] = (Math.Round(rnd.NextDouble() * chance, 13) <= 5) ? biases[i][j] += (float)Math.Round(-val + rnd.NextDouble() * (val + val), 13) : biases[i][j];
            Thread.Yield();
        }
    }

    public int CompareTo(NeuralNetwork other) //Сравнение эффективности
    {
        if (other == null) return 1;

        if (Fitness > other.Fitness)
            return 1;
        else if (Fitness < other.Fitness)
            return -1;
        else
            return 0;
    }

    public NeuralNetwork copy(NeuralNetwork nn) //Создание глубоких копий, чтобы гарантировать сериализацию массивов
    {
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < biases.Length; i++)
        {
            int temp = i;
            NeuralNetwork nc = nn;
            tasks.Add(Task.Factory.StartNew(() => copyLayerBiases(nc, temp)));
        }
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();

        for (int i = 0; i < weights.Length; i++)
        {
            int temp = i;
            NeuralNetwork nc = nn;
            tasks.Add(Task.Factory.StartNew(() => copyLayersWeights(nn, temp)));
        }
        Task.WaitAll(tasks.ToArray());

        return nn;
    }

    private void copyLayersWeights(NeuralNetwork nn, int i)
    {
        for (int j = 0; j < weights[i].Length; j++)
        {
            for (int k = 0; k < weights[i][j].Length; k++)
            {
                nn.weights[i][j][k] = weights[i][j][k];
            }
            Thread.Yield();
        }
    }

    private void copyLayerBiases(NeuralNetwork nn, int i)
    {
        for (int j = 0; j < biases[i].Length; j++)
        {
            nn.biases[i][j] = biases[i][j];
            Thread.Yield();
        }
    }

    public void Load(string path)//Загружает веса и смещения из файла
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }

    public void Save(string path)//Сохраняет веса и смещения в файл
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}