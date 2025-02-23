using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
	[SerializeField] Canvas canvas;

    public List<GameObject> targetObjects = new List<GameObject>();

    List<TargetIndicator> targetIndicators = new List<TargetIndicator>();

	[SerializeField] Camera MainCamera;

    [SerializeField] GameObject TargetIndicatorPrefab;

    // Start is called before the first frame update
    void Start()
    {
		foreach (var target in targetObjects)
		{
			AddTargetIndicator(target.gameObject);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if(targetIndicators.Count > 0)
        {
            for(int i = 0; i < targetIndicators.Count; i++)
            {
				targetIndicators[i].UpdateTargetIndicator();
            }
        }
    }

    public void AddTargetIndicator(GameObject target)
    {
        TargetIndicator indicator = GameObject.Instantiate(TargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
        AddTargetIndicator(indicator, target);
    }

    public void AddTargetIndicator(TargetIndicator indicator, GameObject target)
    {
        indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
        targetIndicators.Add(indicator);
    }

}
