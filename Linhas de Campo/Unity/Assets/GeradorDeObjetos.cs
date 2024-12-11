using UnityEngine;

public class GeradorDeObjetos : MonoBehaviour
{
    public GameObject prefabObjeto;  // Prefab do objeto a ser instanciado
    public Vector3 centroDeGeracao = Vector3.zero;  // Ponto central onde os objetos serão instanciados
    public float raio = 5f;  // Raio do círculo onde os objetos serão instanciados
    private float anguloAtual = 0f;  // Ângulo atual para instanciar o objeto
    private string entradaDeCarga = "1";  // String para armazenar o valor de carga inserido
    private float valorDeCarga = 1.0f;  // Valor de carga convertido da entrada

    void OnGUI()
    {
        // Caixa de texto para inserir o valor de carga
        GUI.Label(new Rect(10, 70, 200, 30), "Valor de Carga:");
        entradaDeCarga = GUI.TextField(new Rect(150, 70, 100, 30), entradaDeCarga);

        // Criação do botão na tela
        if (GUI.Button(new Rect(10, 10, 150, 50), "Instanciar Objeto"))
        {
            // Tenta converter o valor de carga para float
            if (float.TryParse(entradaDeCarga, out valorDeCarga))
            {
                // Instancia o objeto com o valor de carga
                InstanciarObjeto();
            }
            else
            {
                Debug.LogError("Valor de carga inválido!");
            }
        }
    }

    int k = 0;

    void InstanciarObjeto()
    {
        // Calcula a posição do objeto com base no círculo
        Vector3 posicaoDeGeracao = new Vector3(
            centroDeGeracao.x + raio * Mathf.Cos(anguloAtual),
            centroDeGeracao.y,
            centroDeGeracao.z + raio * Mathf.Sin(anguloAtual)
        );

        // Instancia o objeto na posição calculada
        GameObject obj = Instantiate(prefabObjeto, posicaoDeGeracao, Quaternion.identity);
        k++;

        // Chama o método para adicionar a carga ao objeto instanciado
        Camera.main.GetComponent<GerenciadorCampoEletrico>().AdicionarCarga(obj, valorDeCarga);

        // Atualiza o ângulo para o próximo objeto
        anguloAtual += Mathf.PI;  // Aumenta o ângulo para posicionar o próximo objeto na posição oposta
    }
}
