using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

public class DEBUG_U : MonoBehaviour
{
	private void Update()
	{
		if(Input.GetMouseButtonDown(1))
		{
			StopAllCoroutines();
			StartCoroutine(STIMULATE());
		}
	}

	IEnumerator STIMULATE()
	{
		#region frame_rate
		QualitySettings.vSyncCount = 4;
		yield return null; yield return null;
		#endregion

		int[] I = new int[]
		{
			-5, -5, -6, 0, 1, 4, 5, 10, 3, 2, 1,
		};
		// a - b < 0 => b is less than a,so min = b
		int min = U.minMax(I, (a, b) => a - b);

		List<Node> NODE = new List<Node>()
		{
			new Node() { id = "a", dist = 0f, ansc = null  },
		};

		NODE.Add(new Node() { node = NODE[0] ,id = "[0, 1]", dist = 2f, ansc = null    });
		NODE.Add(new Node() { node = NODE[1] ,id = "c", dist = 3f, ansc = NODE[0] });
		NODE.Add(new Node() { node = NODE[1] ,id = "d", dist = 4f, ansc = NODE[0] });
		NODE.RemoveAt(0);
		NODE.Add(new Node() { node = NODE[1] ,id = "e", dist = 4f, ansc = NODE[1] });
		NODE.Add(new Node() { node = NODE[1] ,id = "[1, 2]", dist = 6f, ansc = NODE[1] });

		// log
		//console.log_txt(NODE.toTable());
		//console.log_txt(NODE.get_str());
		//console.log_txt(NODE.toPrimitiveTable());
		console.log_txt(NODE.toTable("NODE LIST<>"));

		Node node = NODE.find(_node => _node.dist == 4f);
		if (node != null) Debug.Log(node.ansc.dist);
		else			  Debug.Log("node = null");

		Dictionary<string, int> doc = new Dictionary<string, int>()
		{
			{"A", 1},
			{"B", 2},
			{"C", 3},
		};
		console.log_txt(doc.toTable("DOC MAP<>"));
	}

	public class Node
	{
		public string id = "";
		public float dist = 0f;
		public Node node;
		public Node ansc = null;
		float s = 0f;

		public override string ToString()
		{
			//return base.ToString();
			return $"dist: {this.dist}, ansc_ref: " + 
						((ansc != null)? this. id : "null");
		}
	}
}
