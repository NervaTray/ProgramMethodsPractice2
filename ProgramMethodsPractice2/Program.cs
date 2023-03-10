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

    public int FIRST(Vertex vertex)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1 == vertex) return edgeList[i].index;
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

    public int NEXT(Vertex vertex, int index)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1 == vertex && edgeList[i].index > index) return edgeList[i].index;
        }

        return 0;
    }
    
    // Возвращает вершину с индексом i из множества вершин, смежных с v.
    public Vertex VERTEX(Vertex vertex, int index)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1 == vertex && edgeList[i].index == index) return edgeList[i].vert2;
        }

        return null;
    }
    
    // Добавляет Вершину.
    public void ADD_V(string name, int mark = 0)
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
        edge.index = 1;
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == begVert && edgeList[i].index >= edge.index) edge.index = edgeList[i].index + 1;
        }
        
        // Добавление созданной дуги в список дуг.
        edgeList.Add(edge);
    }
    
    // Метод удаляет узел.
    public void DEL_V(string vertexName)
    {
        if (findVertex(vertexName) == null) return;
        
        // Удаляет узел из списка узлов.
        for (int i = 0; i < vertexList.Count; i++)
        {
            if (vertexList[i].name == vertexName)
            {
                vertexList.RemoveAt(i);
                break;
            }
        }

        // Удаляет дуги, связанные с удаленным узлом, а также меняет индексы там, где это необходимо.
        int length = edgeList.Count;
        for (int i = 0; i < length; i++)
        {
            if (edgeList[i].vert1.name == vertexName)
            {
                DEL_E(vertexName, edgeList[i].vert2.name);
                length--;
                i--;
                continue;
            }

            if (edgeList[i].vert2.name == vertexName)
            {
                DEL_E(edgeList[i].vert1.name, vertexName);
                length--;
                i--;
            }
        }
    }
    
    // Метод удаляет дугу.
    // Также данный метод меняет индексы у узлов смежных с начальным узлом, удаляемой дуги.
    public void DEL_E(string begVert, string endVert)
    {
        int temp_index = 0;
        int length = edgeList.Count;
        // Удаляет элемент из списка.
        for (int i = 0; i < length; i++)
        {
            if (edgeList[i].vert1.name == begVert && edgeList[i].vert2.name == endVert)
            {
                temp_index = edgeList[i].index;
                edgeList.RemoveAt(i);
                break;
            }
        }
        // В случае если никакой элемент не удалился, значит что подобной дуги нет и можно выходить из метода.
        if ((length - edgeList.Count) == 0) return;
        
        // Меняет индексы.
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].vert1.name == begVert && edgeList[i].index > temp_index) edgeList[i].index--;
        }
    }
    
    // Изменение метки в узле.
    public void EDIT_V(string vertexName, int mark)
    {
        if (findVertex(vertexName) != null) findVertex(vertexName).mark = mark;
    }
    
    // Изменение веса дуги
    public void EDIT_E(string begVert, string endVert, int weight)
    {
        if (findEdge(begVert, endVert) != null) findEdge(begVert, endVert).weight = weight;
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
        if (edgeList.Count == 0)
        {
            Console.WriteLine("Список дуг пуст.");
            return;
        }
        for (int i = 0; i < edgeList.Count; i++)
        {
            Console.WriteLine("{0}. BegVert: (name: {1}, mark: {2})\tEndVert: (name: {3}, mark: {4}, index: {5})\tWeight: {6}", 
                i + 1, edgeList[i].vert1.name, edgeList[i].vert1.mark, edgeList[i].vert2.name, edgeList[i].vert2.mark, edgeList[i].index, edgeList[i].weight);
        }
    }
    
    // Печатает список Узлов
    public void PrintVertexList()
    {
        if (vertexList.Count == 0)
        {
            Console.WriteLine("Список узлов пуст.");
            return;
        }
        for (int i = 0; i < vertexList.Count; i++)
        {
            Console.WriteLine("{0}. Vertex: (name: {1}, mark: {2})", i + 1, vertexList[i].name, vertexList[i].mark);
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
    
    
    // Определение всех циклов в графе методом обхода в ширину.
    public int BFSCyclesCount(Vertex parentVertex, bool output = false)
    {
        // Если узла в списке узлов не существует, то обход в ширину не происходит.
        if (findVertex(parentVertex.name) == null) return -1;
    
        // Выставление всем узлам метки 0.
        // 0 - не пройдено.
        // 1 - посещается.
        // 2 - посещено.
        for (int i = 0; i < vertexList.Count; i++)
            vertexList[i].mark = 0;

        // Счетчик циклов в графе.
        int cycles_count = 0;
        
        // Создание очереди узлов.
        Queue<Vertex> bfs_queue = new Queue<Vertex>();
        
        // Занесение начального узла в очередь.
        bfs_queue.Enqueue(parentVertex);
        bfs_queue.Peek().mark = 1;
        
        // Обход графа.
        while (bfs_queue.Count > 0)
        {
            Vertex v_current = bfs_queue.Peek();
            for (int i = FIRST(v_current); i > 0; i = NEXT(v_current, i))
            {
                Vertex temp = VERTEX(v_current, i);
                if (temp.mark == 0)
                {
                    temp.mark = 1;
                    bfs_queue.Enqueue(temp);
                }
                // В данном случае, если при обходе алгоритм натыкается на пройденный, узел, то
                // начинает проверяться наличие пути от смежного с действующим узлом узла до действующего узло.
                // Если путь существует, значит есть цикл.
                else if ((temp.mark == 2 || temp.mark == 1) && BFSWayExist(temp, v_current))
                {
                    cycles_count++;
                }
            }
            
            v_current.mark = 2;
            // output - true - осуществляется вывод узлов, которые проходит алгоритм.
            // output - false - вывод не осуществляется.
            if (output) PrintVertexInfo(bfs_queue.Dequeue().name);
            else bfs_queue.Dequeue();
        }

        return cycles_count;
    }
    
    
    // Нахождение пути между вершинами через BFS.
    // Если путь между вершинами существует - true.
    // Если путь НЕ существует - false.
    public bool BFSWayExist(Vertex startVertex, Vertex endVertex)
    {
        // Обнуление костыльных меток.
        for (int i = 0; i < vertexList.Count; i++)
            vertexList[i].mark2 = 0;
        
        // Создание очереди и занесение в нее стартового узла.
        Queue<Vertex> bfsQueue = new Queue<Vertex>();
        bfsQueue.Enqueue(startVertex);
        bfsQueue.Peek().mark2 = 1;

        while (bfsQueue.Count > 0)
        {
            Vertex v_current = bfsQueue.Peek();
            
            for (int i = FIRST(v_current); i > 0; i = NEXT(v_current, i))
            {
                Vertex temp = VERTEX(v_current, i);
                if (temp.name == endVertex.name) return true;
                if (temp.mark2 == 0)
                {
                    temp.mark2 = 1;
                    bfsQueue.Enqueue(temp);
                }
            }
            
            v_current.mark2 = 2;
            bfsQueue.Dequeue();
        }
        return false;
    }


    // Конструктор.
    public Graph(bool oriented = true)
    {
        edgeList = new List<Edge>();
        vertexList = new List<Vertex>();
        this.oriented = oriented;
        Vertex nullVertex = new Vertex("NULL", 0);
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
    public int mark;        // Название метки
    public int mark2;       // Костыльная метка

    // Конструктор.
    public Vertex(string name, int mark)
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

        Graph graph2 = new Graph();
        Console.WriteLine("GRAPH 1:\n\n");
        
        graph2.ADD_V("A");
        graph2.ADD_V("B");
        graph2.ADD_V("C");
        graph2.ADD_V("D");
        graph2.ADD_V("E");
        graph2.ADD_V("F");
        
        graph2.ADD_E("A", "B");
        graph2.ADD_E("A", "F");
        graph2.ADD_E("B", "C");
        graph2.ADD_E("C", "A");
        graph2.ADD_E("C", "E");
        graph2.ADD_E("C", "D");
        graph2.ADD_E("F", "D");
        
        graph2.PrintEdgeList();
        Console.Write("Cycles: ");
        Console.WriteLine(graph2.BFSCyclesCount(graph2.findVertex("A")));
        Console.WriteLine();

        Graph graph3 = new Graph();
        Console.WriteLine("GRAPH 2:\n\n");
        
        graph3.ADD_V("A");
        graph3.ADD_V("B");
        graph3.ADD_V("C");
        graph3.ADD_V("D");
        graph3.ADD_V("E");
        graph3.ADD_V("F");
        graph3.ADD_V("G");
        
        graph3.ADD_E("A", "B");
        graph3.ADD_E("B", "C");
        graph3.ADD_E("C", "D");
        graph3.ADD_E("D", "B");
        graph3.ADD_E("A", "E");
        graph3.ADD_E("E", "F");
        graph3.ADD_E("F", "G");
        graph3.ADD_E("G", "E");
        
        graph3.PrintEdgeList();
        Console.Write("Cycles: ");
        Console.WriteLine(graph3.BFSCyclesCount(graph3.findVertex("A")));

    }
}