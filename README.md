# RA-ARvore

Trabalho de Conclusão de Curso desenvolvido por Bruno Geisler Vigentas.  
Tem como o objetivo ajudar no conhecimento de folhas de árvores com o auxílio da Realidade Aumentada.  

Utiliza-se dos seguintes pacotes Unity:

* [Barracuda](https://docs.unity3d.com/Packages/com.unity.barracuda@2.0/manual/index.html) - Para detecção das formas de folhas.
* [ARFoundation](https://unity.com/unity/features/arfoundation) - Para parte de Realidade Aumentada.

## Importação

- Importe e abra o projeto no Unity 2020.2.5f1.
- Ao abrir o projeto, todas as dependências devem ser resolvidas automaticamente. Entretanto pode ocorrer do pacote do Barracuda não ser encontrado. Neste caso faça o download do pacote neste [LINK](https://drive.google.com/file/d/1oFz3Wp8JN8eiXvkbVCddeuJjX7WS7cG9/view?usp=sharing) e o importe manualmente com os passos abaixo:
  - Descompacte o ZIP baixado.
  - No Unity, vá em Windows -> Package Manager, clique no "+" e selecione a opção "Add package from disk".
  - Vá a pasta onde o ZIP foi descompactado e selecione o arquivo "package.json". Feito o pacote está importado!

## Build
- Certifique-se que a plataforma de BUILD selecionada é Android, caso não seja, clique no botão "Switch Plataform" para o Android.
- Em "Players Setting", vá nas opções do Android e verifique as seguintes configurações:
  - Write Permissin: External (SDCard)
- Em "XR Plug-in Managament" verifique as seguintes configurações para Android:
  - Initialize XR on Startup: TRUE
  - ARCore: TRUE
  
 - Clique no botão "Build" para gerar uma APK. 



