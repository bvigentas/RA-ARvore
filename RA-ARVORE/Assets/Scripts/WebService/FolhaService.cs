using System.Net;
using System;
using System.IO;
using UnityEngine;

public class FolhaService : MonoBehaviour
{
    public Folha getArvoreInfo(string nomeCientifico)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(String.Format("https://apirvore.herokuapp.com/api/folha/{0}", nomeCientifico));
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var jsonResponse = reader.ReadToEnd();
            var arvoreInfo = JsonUtility.FromJson<Folha>(jsonResponse);
            return arvoreInfo;
        }
        catch (WebException e)
        {
            var folha = new Folha();
            folha.arvores_folha = "";
            folha.informacoes_folha = "";
            folha.tipo_folha = "";

            return folha;
        }
    }
}
