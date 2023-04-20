using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DetectionSystem : NetworkBehaviour
{
    [Header("Propiedades")]
    private Transform observador;
    public Transform aDetectar;
    [Header("Variables")]
    [Range(1, 100)]
    public float distanciaDeDeteccion;
    public float anguloDeDeteccion;
    public bool localizado;
    [Range(0,10)]
    public float segundosDeteccion;
    [Range(0,10)]
    public float segundosPerdidaAgro;

    private float distanciaEntre;
    private bool perderAgroIniciado, ganarAgroIniciado;

    void Awake()
    {
        this.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            this.enabled = true;
            observador = GetComponent<Transform>();
            localizado = false;
            ganarAgroIniciado = false;
            perderAgroIniciado = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(observador.position, aDetectar.position - observador.position, Color.green);
        Debug.DrawRay(observador.position, Quaternion.Euler(0, anguloDeDeteccion / 2, 0) * observador.forward * distanciaDeDeteccion, Color.red);
        Debug.DrawRay(observador.position, Quaternion.Euler(0, anguloDeDeteccion / -2, 0) * observador.forward * distanciaDeDeteccion, Color.red);

        distanciaEntre = Vector3.Distance(observador.position, aDetectar.position);

        if (distanciaEntre <= distanciaDeDeteccion
            && Vector3.Angle(aDetectar.position - observador.position, observador.forward) <= (anguloDeDeteccion / 2)
            && Physics.Raycast(observador.position, aDetectar.position - observador.position, distanciaDeDeteccion, (int)Mathf.Pow(2, aDetectar.gameObject.layer)))
        {
            if (perderAgroIniciado)
            {
                perderAgroIniciado = false;
                StopCoroutine(PerderAgro());
            }

            if (!ganarAgroIniciado && !localizado)
            {
                ganarAgroIniciado = true;
                StartCoroutine(GanarAgro());
            }
        }
        else
        {
            if (ganarAgroIniciado)
            {
                ganarAgroIniciado = false;
                StopCoroutine(GanarAgro());
            }
            if (!perderAgroIniciado && localizado)
            {
                perderAgroIniciado = true;
                StartCoroutine(PerderAgro());
            }

        }

    }

    IEnumerator GanarAgro()
    {
        yield return new WaitForSeconds(segundosDeteccion);
        ganarAgroIniciado = false;
        localizado = true;
    }

    IEnumerator PerderAgro()
    {
        yield return new WaitForSeconds(segundosPerdidaAgro);
        perderAgroIniciado = false;
        localizado = false;
    }
}
