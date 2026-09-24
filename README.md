# 🛠️ Minecraft Mod Solver (MMS)

Está pensando em criar um servidor, mas o seu modpack está lotado de mods client-side que quebram o console toda vez que você tenta rodá-lo? Cansado de ter que separar e testar os mods manualmente, um por um?

Seus problemas acabaram. O Minecraft Mod Solver (MMS) resolve isso para você. Conectando-se diretamente às APIs oficiais do Modrinth e CurseForge, o aplicativo verifica e organiza os seus arquivos .jar de forma automática, rápida e visual.

Ele separa os mods estritamente de cliente dos mods de servidor, facilitando a movimentação exata do que você precisa para a pasta mods do seu servidor.

## ✨ Recursos

- __Separação Inteligente:__ Divide seus mods automaticamente em subpastas: server side e client side.

- __Isolamento Seguro:__ Não encontrou o mod nas redes? O MMS separa os arquivos não reconhecidos em uma pasta non verified. Assim, seu servidor não quebra e você pode decidir manualmente se aquele mod específico deve ou não entrar.

- __Integração Dupla:__ Consulta o ecossistema do Modrinth (padrão) e possui suporte completo ao banco de dados do CurseForge.

- __Geração de Relatório:__ Cria automaticamente um arquivo modslist.json com o histórico do que foi avaliado para consultas futuras.

- __Interface Gráfica (GUI):__ Construído em C# e Avalonia UI, oferecendo uma tela fluida, limpa e com acompanhamento em tempo real.

## ⚙️ Como funciona?

Cada arquivo de modificação (mod) presente na sua pasta possui uma assinatura digital única chamada Hash (como uma impressão digital do arquivo).

Ao executar o Minecraft Mod Solver, o aplicativo lê os arquivos e calcula instantaneamente esse hash. Em seguida, ele consulta os bancos de dados do Modrinth (via SHA-1) e do CurseForge (via Murmur2). Ao encontrar o arquivo na rede, a API devolve os metadados oficias do desenvolvedor, incluindo o ambiente de aplicabilidade daquele mod (client, server ou both), permitindo que o MMS faça a triagem perfeita.

## ⚠️ Atenção: API do CurseForge

A busca no Modrinth é nativa, gratuita e aberta, funcionando perfeitamente logo ao abrir o aplicativo.

No entanto, para que o aplicativo consiga buscar mods exclusivos do CurseForge, é obrigatório o uso de uma Chave de API (API Key) pessoal.

Como obter e usar a chave:

    Acesse o Console do CurseForge.

    Crie uma conta/faça login e gere uma chave de API gratuita.

    Abra o MMS e cole a chave no campo "CurseForge API Key".

    Clique em Salvar e depois em Testar API. O MMS guardará essa chave no seu sistema (no %AppData%) para que você não precise digitá-la novamente nas próximas vezes.

(Se a chave não for fornecida, o aplicativo funcionará normalmente, mas fará buscas apenas no banco de dados do Modrinth).

## 🚀 Como Usar

    Baixe a última versão executável na aba Releases.

    Abra o Minecraft Mod Solver.

    Clique em Procurar Pasta... e selecione a pasta onde estão os mods originais do seu modpack.

    (Opcional) Insira sua chave do CurseForge para ampliar a busca.

    Clique em Verificar e Organizar Mods.

    Pronto! O aplicativo criará uma subpasta chamada modVerifier Files dentro do seu diretório original, contendo todos os mods perfeitamente separados e prontos para uso no seu servidor.

Desenvolvido para facilitar a vida de donos de servidores e criadores de modpacks. ☕🧊