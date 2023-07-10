<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Signer Plenus</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@0.9.0/css/bulma.min.css">
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
</head>
<body>
    <div class="container">
        <div class="card">
            <header class="card-header">
                <p class="card-header-title">
                    Signer Plenus
                </p>
            </header>
            <div class="card-content has-text-centered">
                <p class="subtitle">Assinatura Digital</p>
                <input type="file" name="" id="file">
                <div class="content" >
                    <button class="button is-info actived" id="btn-lc">
                        <i class="material-icons">assignment_turned_in</i>
                    </button>
                </div>
            </div>
        </div>
        <footer class="card-footer">
            <p class="card-footer-item">Aplicação para assinatura digital A3
            <?= $_SERVER['REMOTE_ADDR'] ?>
            </p>
        </footer>
    </div>
    <script>
        var ws = new WebSocket('ws://localhost:2514');

        var f = new Blob([
            document.querySelector('#file').files[0],
            {
                'content-type':'application/json'
            }
        ]);

        var a = new FileReader();

        ws.send({

        })

    </script>
</body>
</html>
