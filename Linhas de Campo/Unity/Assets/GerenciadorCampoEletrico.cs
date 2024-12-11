using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Necessário para trabalhar com cenas

public class GerenciadorCampoEletrico : MonoBehaviour
{
    [System.Serializable]
    public class Carga {
        public Vector3 posicaoOriginal;
        public Vector3 posicaoAtual;
        public float valorCarga; // Carga positiva ou negativa
        private GameObject cargaGameObject;

        public void SetCargaGameObject(GameObject obj) {
            this.cargaGameObject = obj;
            this.posicaoOriginal = obj.transform.position;
            this.posicaoAtual = this.posicaoOriginal;
        }

        public GameObject GetCargaGameObject() {
            return this.cargaGameObject;
        }
    }

    public GameObject cargaSelecionada = null;
    public GameObject prefabCarga;
    public List<Carga> cargas = new List<Carga>(); // Lista de cargas
    public int linhasPorCarga = 6; // Número de linhas de campo por carga
    public float tamanhoPasso = 0.1f; // Tamanho do passo para o cálculo das linhas de campo
    public float distanciaMaxima = 20.0f; // Distância máxima para as linhas de campo
    public GameObject prefabLinhaCampo; // Prefab para visualizar as linhas de campo
    GameObject objetoLinha;
    int k = 1;

    void Start() {
        foreach (var carga in cargas) {
            GameObject novaCarga = GameObject.Instantiate(prefabCarga, carga.posicaoOriginal, Quaternion.identity);
            novaCarga.name = "Carga " + k.ToString();
            carga.SetCargaGameObject(novaCarga);
            CriarLinhasCampo(carga);
            k++;
        }
    }

    void Update() {
        foreach (var carga in cargas) {
            carga.posicaoAtual = carga.GetCargaGameObject().transform.position;
            if (carga.posicaoAtual != carga.posicaoOriginal) {
                Debug.Log(carga.posicaoAtual);
                foreach (Transform filho in carga.GetCargaGameObject().transform) {
                    Destroy(filho.gameObject);
                }
                CriarLinhasCampo(carga);
            }
        }
    }

    void CriarLinhasCampo(Carga carga)
    {
        for (int i = 0; i < linhasPorCarga; i++)
        {
            float angulo = (2 * Mathf.PI / linhasPorCarga) * i;
            Vector3 posicaoInicial = carga.posicaoAtual + new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0) * 0.1f;
            GerarLinhaCampo(posicaoInicial, carga);
        }
    }

    void GerarLinhaCampo(Vector3 posicaoInicial, Carga carga) {
        Vector3 posicao = posicaoInicial;
        Vector3 direcao = Vector3.zero;

        objetoLinha = Instantiate(prefabLinhaCampo, transform);
        LineRenderer linhaRenderer = objetoLinha.GetComponent<LineRenderer>();
        linhaRenderer.positionCount = 1;
        linhaRenderer.SetPosition(0, posicao);
        objetoLinha.transform.parent = carga.GetCargaGameObject().transform;

        while (Vector3.Distance(posicao, posicaoInicial) < distanciaMaxima)
        {
            Vector3 campo = CalcularCampoEletrico(posicao);
            if (campo.magnitude < 1e-4f) break; // Para se o campo for negligível

            direcao = (carga.valorCarga > 0 ? 1 : -1) * campo.normalized * tamanhoPasso;
            posicao += direcao;

            linhaRenderer.positionCount++;
            linhaRenderer.SetPosition(linhaRenderer.positionCount - 1, posicao);
        }
    }

    Vector3 CalcularCampoEletrico(Vector3 ponto)
    {
        Vector3 campoTotal = Vector3.zero;

        foreach (var carga in cargas)
        {
            Vector3 deslocamento = ponto - carga.posicaoAtual;
            float distanciaQuadrada = deslocamento.sqrMagnitude;
            if (distanciaQuadrada > 1e-6f) // Evita divisão por zero
            {
                float magnitudeCampo = carga.valorCarga / distanciaQuadrada;
                campoTotal += deslocamento.normalized * magnitudeCampo;
            }
        }

        return campoTotal;
    }

    public void AdicionarCarga(GameObject cargaObjeto, float valorCarga) {
        // Cria uma nova instância de Carga
        Carga novaCarga = new Carga();
        novaCarga.posicaoOriginal = cargaObjeto.transform.position;
        novaCarga.valorCarga = valorCarga;
        novaCarga.posicaoAtual = novaCarga.posicaoOriginal;

        // Define o GameObject para a Carga
        novaCarga.SetCargaGameObject(cargaObjeto);

        // Adiciona a nova carga à lista
        cargas.Add(novaCarga);

        // Opcionalmente, você pode criar linhas de campo para a nova carga
        CriarLinhasCampo(novaCarga);
    }

    public void AlternarLinhasCampo(GameObject cargaObjeto)
    {
        Carga carga = cargas.Find(c => c.GetCargaGameObject() == cargaObjeto);
        if (carga != null)
        {
            // Alterna a visibilidade das linhas de campo
            foreach (Transform filho in cargaObjeto.transform)
            {
                filho.gameObject.SetActive(!filho.gameObject.activeSelf);
            }
        }
    }

    public void DeletarCarga(GameObject cargaObjeto)
    {
        // Encontra e remove a carga da lista
        Carga cargaParaRemover = cargas.Find(c => c.GetCargaGameObject() == cargaObjeto);
        if (cargaParaRemover != null)
        {
            cargas.Remove(cargaParaRemover);
            Destroy(cargaParaRemover.GetCargaGameObject());
            // Opcionalmente, também destrói as linhas de campo associadas à carga
            foreach (Transform filho in cargaObjeto.transform)
            {
                Destroy(filho.gameObject);
            }
        }
    }

    public void DuplicarCarga(GameObject cargaObjeto)
    {
        Carga cargaParaDuplicar = cargas.Find(c => c.GetCargaGameObject() == cargaObjeto);
        if (cargaParaDuplicar != null)
        {
            // Duplica a carga
            GameObject novoObjetoCarga = Instantiate(prefabCarga, cargaParaDuplicar.posicaoOriginal, Quaternion.identity);
            AdicionarCarga(novoObjetoCarga, cargaParaDuplicar.valorCarga);
        }
    }

    public void AtualizarValorCarga(GameObject cargaObjeto, float novoValorCarga)
    {
        Carga cargaParaAtualizar = cargas.Find(c => c.GetCargaGameObject() == cargaObjeto);
        if (cargaParaAtualizar != null)
        {
            cargaParaAtualizar.valorCarga = novoValorCarga;
            // Atualiza as linhas de campo elétrico, se necessário
            foreach (Transform filho in cargaObjeto.transform)
            {
                Destroy(filho.gameObject);
            }
            CriarLinhasCampo(cargaParaAtualizar);
        }
    }

    public void RecarregarCena() {
        // Obter o nome da cena atual
        string cenaAtual = SceneManager.GetActiveScene().name;

        // Recarregar a cena atual
        SceneManager.LoadScene(cenaAtual);
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(165,10,200,50),"Resetar tudo")) {
            RecarregarCena();
        }
        if (cargaSelecionada != null)
        {
            GUI.BeginGroup(new Rect(280, 10, 300, 250));

            // Rótulo com o nome da carga selecionada
            GUI.Label(new Rect(10, 10, 200, 30), "Carga Selecionada: " + cargaSelecionada.name);

            // Botão para deletar a carga selecionada
            if (GUI.Button(new Rect(10, 40, 200, 30), "Deletar Carga"))
            {
                DeletarCarga(cargaSelecionada);
                cargaSelecionada = null; // Limpa a seleção após deletar
            }

            // Botão para duplicar a carga selecionada
            if (GUI.Button(new Rect(10, 70, 200, 30), "Duplicar Carga"))
            {
                DuplicarCarga(cargaSelecionada);
            }

            GUI.EndGroup();
        }
    }
}
