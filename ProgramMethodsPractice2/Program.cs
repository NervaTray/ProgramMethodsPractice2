using System;
using System.Text.Json;


// Реализация АТД граф.
class Graph
{
    private List<Edge> edgeList;            // Список дуг.
    private List<Vertex> vertexList;        // Список существующий узлов.

    private bool oriented;
    private int index;
    
    
    // Добавляет Вершину.
    public void ADD_V(string name, string mark)
    {
        // Проверка на нулевой элемент.
        // В случае, если пользователь пытается добавить элемент NULL, появляется ошибка.
        if (checkNull(name)) throw new GraphException("Attempted to add NULL vertex.", name);
        
        // Проверяет существует ли узел с данным именем.
        // Если узел с данным именем существует, то новый узел не добавляется.
        // Исключение при этом не выбрасывается.
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (name == vertexList[i].name) return;
        }
        
        // Создание нового узла и его добавление в список узлов.
        Vertex vertex = new Vertex(name, mark, index);
        vertexList.Add(vertex);
        // Увеличение счетчика индекса для узлов.
        index++;
    }
    
    // Добавляет Дугу.
    // Первый аргумент принимает название начального узла.
    // Второй аргумент принимает название конечного узла.
    public void ADD_E(string begVert, string endVert, int weight = 0)
    {
        // Проверка на нулевой элемент.
        // При попытки создания дуги с узлом NULL выбрасывается исключение.
        if (checkNull(begVert) || checkNull(endVert)) throw new GraphException("Attempted to make edge with NULL vertex.", "NULL");
            
        Edge edge = new Edge();
        edge.weight = weight;
        // Проверка на существование данных узлов.
        // Если введенных узлов не существует, то новая дуга не заносится в список дуг.
        // Параллельно с проверкой заносятся данные в узел.
        bool flag = false;
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (vertexList[i].name == begVert)
            {
                flag = true;
                edge.vert1 = vertexList[i];
                break;
            }
        }
        if (!flag) return;
        flag = false;
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (vertexList[i].name == endVert)
            {
                flag = true;
                edge.vert2 = vertexList[i];
                break;
            }
        }
        if (!flag) return;
        
        edgeList.Add(edge);
    }
    
    
    // Печатает инфо по узлу.
    public void PrintVertexInfo(string name)
    {
        Vertex temp = findVertex(name);
        if (temp == null) return;
    }
    
    
    // Печатает список дуг
    public void PrintEdgeList()
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            Console.WriteLine("{0}. BegVert: (name: {1}, mark: {2}, index: {3})\tEndVert: (name: {4}, mark: {5}, index: {6})\tWeight: {7}", 
                i + 1, edgeList[i].vert1.name, edgeList[i].vert1.mark, edgeList[i].vert1.Index, edgeList[i].vert2.name, edgeList[i].vert2.mark, edgeList[i].vert2.Index, edgeList[i].weight);
        }
    }
    
    
    // Проверяет является ли узел "нулевым".
    // Возвращает true если элемент нулевой, в противном случае возвращает false.
    private bool checkNull(string check)
    {
        if (check.ToLower() == "null") return true;
        return false;
    }

    // Поиск узла.
    private Vertex findVertex(string name)
    {
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (vertexList[i].name == name) return vertexList[i];
        }

        return null;
    }
    
    // Поиск дуги.
    //private Edge findEdge()


    // Конструктор.
    public Graph(bool oriented = true)
    {
        edgeList = new List<Edge>();
        vertexList = new List<Vertex>();
        this.oriented = oriented;
        index = 1;
        Vertex nullVertex = new Vertex("NULL", "NULL", 0);
        vertexList.Add(nullVertex);
    }
}


// Реализация дуги.
class Edge
{
    public Vertex vert1;        // Начальный узел
    public Vertex vert2;        // Конечный узел
    public int weight;          // Вес дуги

}


// Реализация узла.
class Vertex
{
    public string name;     // Имя узла
    public string mark;     // Название метки
    private int index;      // Индекс узла

    public int Index => index;

    // Конструктор.
    public Vertex(string name, string mark, int index)
    {
        this.name = name;
        this.mark = mark;
        this.index = index;
    }
}


class GraphException : ArgumentException
{
    public string Value;
    public GraphException(string message, string value) : base(message)
    {
        this.Value = value;
    }
}


class Program
{
    static void Main()
    {
        Graph graph = new Graph();

        try
        {
            graph.ADD_V("NULL", "m");
        }
        catch (GraphException ex)
        {
            Console.WriteLine(ex.Value);
            Console.WriteLine(ex.Message);
        }
        
        
        // graph.ADD_V("A", "m");
        // graph.ADD_V("B", "m");
        // graph.ADD_V("C", "m");
        // graph.ADD_V("D", "m");
        //
        // graph.ADD_E("A", "B");
        // graph.ADD_E("A", "C");
        // graph.ADD_E("B", "D");
        // graph.ADD_E("C", "D");
        //
        // graph.PrintEdgeList();
    }
}