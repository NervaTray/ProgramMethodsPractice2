using System;
using System.Text.Json;

// Поработать со значением null.

// Реализация АТД граф.
class Graph
{
    private List<Edge> edgeList;            // Список дуг.
    private List<Vertex> vertexList;        // Список существующий узлов.

    private bool oriented;                  // true - ор.граф, false - не ор.граф.


    // Возвращает индекс первой вершины, смежной с введенной вершиной.
    // Если вершина не имеет смежных вершин, то возвращается нулевая вершина.
    public int FIRST(string vertexName)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == vertexName) return edgeList[i].index;
        }

        return 0;
    }
    
    // Возвращает индекс вершины, смежной с вершиной v, следующий за индексом i.
    // Если i — это индекс последней вершины, смежной с вершиной v, то возвращается 0.
    public int NEXT(string vertexName, int index)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == vertexName && edgeList[i].index > index) return edgeList[i].index;
        }

        return 0;
    }
    
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
        Vertex vertex = new Vertex(name, mark);
        vertexList.Add(vertex);
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
        // Параллельно с проверкой заносятся данные в узлы, находящиеся в объекте класса дуги.
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
        // В случае ненахождения узла с заданным именем, выбрасывается исключение.
        if (!flag) throw new GraphException("Attempted to add edge with non-existent Vertex", begVert);
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
        // В случае ненахождения узла с заданным именем, выбрасывается исключение.
        if (!flag) throw new GraphException("Attempted to add edge with non-existent Vertex", endVert);

        // В случае если данная дуга уже существует, она не добавляется в список дуг, при этом исключение не выкидывается.
        if (findEdge(begVert, endVert) != null) return; 
        
        // Назначение индекса для смежного узла
        edge.index = 0;
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == begVert && edgeList[i].index >= edge.index) edge.index = edgeList[i].index + 1;
        }
        
        // Добавление созданной дуги в список дуг.
        edgeList.Add(edge);
    }
    
    // Печатает инфо по узлу.
    public void PrintVertexInfo(string name)
    {
        Vertex temp = findVertex(name);
        if (temp == null)
        {
            Console.WriteLine("Данного узла не существует.");
            return;
        }
        
        Console.WriteLine("Vertex: (name: {0}, mark: {1})", temp.name, temp.mark);
    }
    
    // Печатает инфо по дуге.
    public void PrintEdgeInfo(string begVert, string endVert)
    {
        Edge temp = findEdge(begVert, endVert);
        if (temp == null)
        {
            Console.WriteLine("Данной дуги не существует.");
            return;
        }
        
        Console.WriteLine("BegVert: (name: {0}, mark: {1})\tEndVert: (name: {2}, mark: {3}, index: {4})\tWeight: {5}",
            temp.vert1.name, temp.vert1.mark, temp.vert2.name, temp.vert2.mark, temp.index, temp.weight);
    }
    
    // Печатает список дуг
    public void PrintEdgeList()
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            Console.WriteLine("{0}. BegVert: (name: {1}, mark: {2})\tEndVert: (name: {3}, mark: {4}, index: {5})\tWeight: {6}", 
                i + 1, edgeList[i].vert1.name, edgeList[i].vert1.mark, edgeList[i].vert2.name, edgeList[i].vert2.mark, edgeList[i].index, edgeList[i].weight);
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
    // Если узел не находится, то возвращается null.
    public Vertex findVertex(string name)
    {
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (vertexList[i].name == name) return vertexList[i];
        }

        return null;
    }
    
    // Поиск дуги.
    // Если дуга не находится, то возвращается null.
    public Edge findEdge(string begVert, string endVert)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == begVert && edgeList[i].vert2.name == endVert) return edgeList[i];
        }

        return null;
    }


    // Конструктор.
    public Graph(bool oriented = true)
    {
        edgeList = new List<Edge>();
        vertexList = new List<Vertex>();
        this.oriented = oriented;
        Vertex nullVertex = new Vertex("NULL", "NULL");
        vertexList.Add(nullVertex);
    }
}


// Реализация дуги.
class Edge
{
    public Vertex vert1;        // Начальный узел
    public Vertex vert2;        // Конечный узел
    public int weight;          // Вес дуги
    public int index;           // индекс смежного узла

}


// Реализация узла.
class Vertex
{
    public string name;     // Имя узла
    public string mark;     // Название метки

    // Конструктор.
    public Vertex(string name, string mark)
    {
        this.name = name;
        this.mark = mark;
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
        
        graph.ADD_V("A", "m");
        graph.ADD_V("B", "m");
        graph.ADD_V("C", "m");
        graph.ADD_V("D", "m");
        
        graph.ADD_E("A", "B");
        graph.ADD_E("A", "C");
        graph.ADD_E("B", "D");
        graph.ADD_E("C", "D");

        graph.PrintEdgeList();
        graph.PrintEdgeInfo("A", "H");
        graph.PrintEdgeInfo("A", "C");
        graph.PrintVertexInfo("A");
        graph.PrintVertexInfo("G");
      

        Console.WriteLine(graph.FIRST("A"));
        Console.WriteLine(graph.NEXT("A", 0));
    }
}