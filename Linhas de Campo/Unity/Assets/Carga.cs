using UnityEngine;

public class MoveSphereUI : MonoBehaviour {
    public bool estaSelecionado = false;
    private float novoValorDeCarga = 0f;  // Variável para armazenar o novo valor de carga

    void Start() {
        GetComponent<Renderer>().material.color = Color.white;
    }

    void OnMouseEnter() {
        if (!estaSelecionado && Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada == null) {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    void OnMouseExit() {
        if (!estaSelecionado && Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada == null) {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    void OnMouseDown() {
        if (Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada == null || Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada == gameObject) {
            estaSelecionado = !estaSelecionado;
            Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada = estaSelecionado ? gameObject : null;
            GetComponent<Renderer>().material.color = estaSelecionado ? Color.red : Color.white;
        }
    }

    void OnGUI() {
        if (estaSelecionado && Camera.main.GetComponent<GerenciadorCampoEletrico>().cargaSelecionada == gameObject) {
            GUI.BeginGroup(new Rect(10, 10, 300, 250));

            GUI.Label(new Rect(10, 100, 200, 30), gameObject.name);
            GUI.Label(new Rect(10, 130, 200, 30), "Posição Atual:");
            GUI.Label(new Rect(10, 160, 200, 30), "X: " + gameObject.transform.position.x.ToString("F2"));
            GUI.Label(new Rect(10, 190, 200, 30), "Y: " + gameObject.transform.position.y.ToString("F2"));
            GUI.Label(new Rect(10, 220, 200, 30), "Z: " + gameObject.transform.position.z.ToString("F2"));

            // Botão para deletar a carga
            if (GUI.Button(new Rect(10, 250, 200, 30), "Deletar Carga")) {
                Camera.main.GetComponent<GerenciadorCampoEletrico>().DeletarCarga(gameObject);
            }

            // Botão para duplicar a carga
            if (GUI.Button(new Rect(10, 280, 200, 30), "Duplicar Carga")) {
                Camera.main.GetComponent<GerenciadorCampoEletrico>().DuplicarCarga(gameObject);
            }

            // Campo para inserir o novo valor de carga
            GUI.Label(new Rect(10, 310, 200, 30), "Novo Valor de Carga:");
            novoValorDeCarga = float.Parse(GUI.TextField(new Rect(150, 310, 100, 30), novoValorDeCarga.ToString()));

            // Botão para atualizar o valor da carga
            if (GUI.Button(new Rect(10, 340, 200, 30), "Atualizar Valor de Carga")) {
                Camera.main.GetComponent<GerenciadorCampoEletrico>().AtualizarValorCarga(gameObject, novoValorDeCarga);
            }

            // Botão para alternar a visibilidade das linhas de campo elétrico
            if (GUI.Button(new Rect(10, 370, 200, 30), "Alternar Linhas de Campo")) {
                Camera.main.GetComponent<GerenciadorCampoEletrico>().AlternarLinhasCampo(gameObject);
            }

            GUI.EndGroup();
        }
    }
}
