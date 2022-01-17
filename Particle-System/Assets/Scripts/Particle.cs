using System;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector3 Velocity { get; set; }
    public Color StartColor { get; set; }
    public Color EndColor { get; set; }

    public DateTime BirthTime { get; set; }
    public DateTime DeathTime { get; set; }

    private SpriteRenderer _spriteRenderer;
    private Camera _camera;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
    }

    private void Update()
    {
        var duration = (DeathTime - BirthTime).TotalMilliseconds;
        var current = (DateTime.Now - BirthTime).TotalMilliseconds;
        var t = (float) (current / duration);

        _spriteRenderer.color = Color.Lerp(StartColor, EndColor, t);

        transform.position += Velocity * Time.deltaTime;
        transform.localScale = new Vector3(1 - t, 1 - t, 1 - t);
        transform.LookAt(_camera.transform.position, -Vector3.up);
    }
}
