
import '../css/pagescss/ConvertToDocs.css'

export default function ConvertToDocs(){
    const sendApi = async (event) => {
        event.preventDefault(); // Previne o comportamento padrão do formulário

        const formData = new FormData();
        const fileInput = event.target.elements.fileInput.files[0];
        formData.append('file', fileInput);

        try {
            const response = await fetch("http://localhost:5179/CovertToDocs", {
                method: 'POST',
                body: formData, // Envia o arquivo usando FormData
            });

            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);

                // Tenta obter o nome do arquivo a partir do cabeçalho 'Content-Disposition'
                const contentDisposition = response.headers.get('Content-Disposition');
                let fileName = 'download.csv'; // Nome de arquivo padrão

                if (contentDisposition) {
                    const fileNameMatch = contentDisposition.split('filename=')[1];
                    if (fileNameMatch) {
                        fileName = fileNameMatch.replace(/['"]/g, ''); // Remove aspas, se existirem
                    }
                }

                const a = document.createElement('a');
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                a.remove();
            } else {
                const errorText = await response.text();
                console.error('Erro:', errorText);
            }
        } catch (error) {
            console.error('Erro:', error);
        }
    };

    return (
        <div className="form-container">
            <form onSubmit={sendApi}>
                <input type='file' name="fileInput" />
                <p>Coverter PDF para Docs</p>
                <button type="submit">Enviar</button>
            </form>
        </div>
    );
}