using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingRoad : MonoBehaviour
{
    #region FIELDS

    [SerializeField]
    public float _moveSpeed = 3.0f;

    [SerializeField]
    private float startZ = 0f;

    [SerializeField]
    private float endZ = 10f;

	[SerializeField] bool usePhysic = false;

    #endregion

    private Rigidbody _rigidbody;

    #region MONOBEHAVIOUR

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
    }

    public void FixedUpdate()
    {
        if(Mathf.Abs(transform.localPosition.z - endZ) > 0.1f)
        {
			if(usePhysic)
			{
				_rigidbody.MovePosition(transform.position - transform.forward * _moveSpeed * Time.deltaTime);
			}
            else
			{
				transform.position = transform.position - transform.forward * _moveSpeed * Time.deltaTime;
			}
		} else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, startZ);
        }
    }

	#endregion
}
