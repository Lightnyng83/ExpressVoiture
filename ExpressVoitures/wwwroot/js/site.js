
$(document).ready(function() {
    $("#SelectedBrand").select2({
        placeholder: "Sélectionnez une marque ou tapez en une nouvelle",
        tags: true, // Autorise la saisie libre d'une valeur non existante
        tokenSeparators: [',', ' ']
    });
});

$(document).ready(function() {
    $("#SelectedModel").select2({
        placeholder: "Sélectionnez un modèle ou tapez en un nouveau",
        tags: true, // Autorise la saisie libre
        tokenSeparators: [',', ' ']
    });
});
