# Force-Directed-Graph-Unity-Neo4j

Demo Video on Youtube: 
[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/gEigfzGDBM4/0.jpg)](http://www.youtube.com/watch?v=gEigfzGDBM4)


# Testing:

1. Install [Neo4j Desktop](https://neo4j.com/download/)
2. Open Unity (ForceDirectedGraphUnity)
3. check server setting variables in ServerConnector on Graph Object (neo4j ip,user,pwd...etc)
4. hit RUN!


# Structue:

# NeoUnity: VS Folder
defination of Neo4j data class such like Nodes and Relationships
 - Server.cs : connect to Neo4j server and put data value in Node and Relationship class

it produced NeoUnity.dll for unity project below.

# ForceDirectedGraph: Unity Folder
Render force directed graph by neo4j movie data


Frontend(On Graph Object in Scene):
 - ServerConnector.cs: setup data entity(movie and person) , server settings.
 - GraphRenderer.cs  : setup UI , Graph settings(relationship line color) , Camera.
 new Graph and GraphScene in Awake(). and create Nodes and Edges(Relationship) while while receiving the neo4j data.
 
Backend:

 - SimpleGraphBackend.cs : extend AbstractGraphBackend.cs Interface.
 - SimpleGraphEdge.cs : extend AbstractGraphEdge.cs Interface.
 - SimpleGraphNode.cs : extend AbstractGraphNode.cs Interface.
 - Graph.cs : extend GraphBackendListener.cs .define NewNode and NewEdge
 - GraphScene.cs : set nodes distance by updating SpringJoint Rigidbody SphereCollider component 
 - NodeComponent.cs : extend AbstractSceneComponent.cs Interface. Initial Node gameObject.
 - EdgeComponent.cs : extend AbstractSceneComponent.cs Interface. Set LineRenderer between 2 Node gameObjects.
 - SpringLayout : extend AbstractGraphLayout.cs Interface. Calculate force directed physics between nodes. 
