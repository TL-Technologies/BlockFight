
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicFollow : MonoBehaviour
{
    #region FIELDS

    [SerializeField]
    public float _moveSpeed = 3.0f;

    [SerializeField]
    private GameObject[] _targets;

    [SerializeField]
    private float minRefreshTime = 3f;
    [SerializeField]
    private float maxRefreshTime = 6f;

    #endregion

    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;
    private GameObject _target;
    private Vector3 _noiseMove;
    private float refreshTargetTime;

    #region MONOBEHAVIOUR

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        RandomTarget();
        refreshTargetTime = Time.timeSinceLevelLoad + Random.Range(minRefreshTime, maxRefreshTime);
    }

    public void FixedUpdate()
    {
        if(_target == null)
        {
            return;
        }
        _moveDirection = _target.transform.position - transform.position;
        _noiseMove = new Vector3(Mathf.PerlinNoise(Time.time, 0), 0, Mathf.PerlinNoise(0, Time.time));
        if(_moveDirection.magnitude > 0.1f)
        {
            _rigidbody.MovePosition(transform.position + (_moveDirection.normalized + _noiseMove) * _moveSpeed * Time.deltaTime);
        } else
        {
            _rigidbody.MovePosition(transform.position + _noiseMove * _moveSpeed * Time.deltaTime);
        }

        if(Time.timeSinceLevelLoad > refreshTargetTime)
        {
            refreshTargetTime = Time.timeSinceLevelLoad + Random.Range(minRefreshTime, maxRefreshTime);
            RandomTarget();
        }
    }

    void RandomTarget()
    {
        if(_targets.Length <= 0)
        {
            return;
        }
        _target = _targets[Random.Range(0, _targets.Length)];
    }

    #endregion
}
