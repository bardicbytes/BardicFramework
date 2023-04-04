using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using BardicBytes.BardicFramework;
using System.Collections;

public class NeuralNode : MonoBehaviour
{
    public float connectionRadius = 10f;
    public int maxConnections = 3;
    public LineRenderer connectionTemplate;
    public ActorTag tagTarget;

    private List<NeuralNode> connections = new List<NeuralNode>();
    private List<LineRenderer> connectionRenderers = new List<LineRenderer>();
    private List<float> weights = new List<float>();
    private float output = 0f;

    private void Awake()
    {
        connectionTemplate.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (var lr in connectionRenderers)
        {
            Destroy(lr.gameObject);
        }
        connectionRenderers.Clear();
    }

    private void Update()
    {
        CheckConnections();

        for (int i = 0; i < connections.Count; i++)
        {
            connectionRenderers[i].SetPosition(0, transform.position);
            connectionRenderers[i].SetPosition(1, connections[i].transform.position);
        }
    }

    private void CheckConnections()
    {
        var nearbyNodes = tagTarget.GetTaggedModules<NeuralNode>();
        for (int i = 0; i < connections.Count; i++)
        {
            if ((transform.position - connections[i].transform.position).sqrMagnitude > connectionRadius * connectionRadius)
            {
                //Remove connection that are too far
                Destroy(connectionRenderers[i].gameObject);
                connections.RemoveAt(i);
                connectionRenderers.RemoveAt(i);
                weights.RemoveAt(i);
                i--;
            }
        }
        // Find new nearby NeuralNodes
        foreach (NeuralNode node in nearbyNodes)
        {
            if (!connections.Contains(node) && (transform.position - node.transform.position).sqrMagnitude <= connectionRadius * connectionRadius && node != this && weights.Count <= maxConnections)
            {
                // Create new connection to node
                connections.Add(node);

                // Create LineRenderer to visualize connection
                LineRenderer renderer = Instantiate(connectionTemplate, transform);
                renderer.SetPosition(0, transform.position);
                renderer.SetPosition(1, node.transform.position);
                connectionRenderers.Add(renderer);
                renderer.gameObject.SetActive(true);

                // Assign random weight to connection
                weights.Add(Random.Range(-1f, 1f));
            }
        }
    }

    [ContextMenu("StartFeed")]
    public void StartFeed() => FeedForward(Random.value);

    public async Task FeedForward(float input)
    {
        if (!this.enabled) return;
        Debug.Log("feed forward "+input,this);
        output = input;
        for (int i = 0; i < connections.Count; i++)
        {
            output += connections[i].output * weights[i];
            connectionRenderers[i].startWidth = Mathf.Abs(weights[i]);
            connectionRenderers[i].endWidth = Mathf.Abs(weights[i]);
            connectionRenderers[i].SetPosition(0, transform.position);
            connectionRenderers[i].SetPosition(1, connections[i].transform.position);

            await connections[i].FeedForward(output);
        }
    }
}