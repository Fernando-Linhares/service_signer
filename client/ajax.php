<?php

$data = base64_encode(file_get_contents("/home/fernando/projects/signer/client/myfile000.pdf"));

http_response_code(200);

header('content-type: application/json');

echo json_encode([
    'data' => $data
]);

die;