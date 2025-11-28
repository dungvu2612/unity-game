using UnityEngine;

public class EnemyBullet : MonoBehaviour{
    
[Header("Bullet Settings")][SerializeField] public float damage = 10f; 
[SerializeField] private float speed = 8f; 
[SerializeField] private float lifetime = 4f;
    private Vector3 dir; 
    private void Start() { 
        Destroy(gameObject, lifetime); 
    } 
    private void Update() { 
        if (dir == Vector3.zero) return; transform.position += dir * Time.deltaTime; 
    } 
    public void SetDirection(Vector3 direction) { 
        dir = direction;
    } }