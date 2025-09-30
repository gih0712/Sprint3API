using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sprint3API; 

// Classe base para respostas com links HATEOAS
public class ResourceResponse
{
    public Dictionary<string, Link> Links { get; set; } = new();
}

// Representa um link HATEOAS
public class Link
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

// Resposta paginada para listas
public class PagedResponse<T>
{
    public T Data { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PagedResponse(T data, int total, int page, int pageSize)
    {
        Data = data;
        TotalCount = total;
        Page = page;
        PageSize = pageSize;
    }
}

public class Moto
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Corrigido: adicionado string.Empty
    public DateTime DataEntrada { get; set; } = DateTime.Now;
    public int TempoLimite { get; set; }
}

// Requisição para criar uma moto
public class CreateMotoRequest
{
    [Required(ErrorMessage = "Placa é obrigatória")]
    public string Placa { get; set; } = string.Empty;
    [Required(ErrorMessage = "Cor é obrigatória")]
    public string Cor { get; set; } = string.Empty;
    [Required(ErrorMessage = "Status é obrigatório")]
    public string Status { get; set; } = string.Empty;
    public int TempoLimite { get; set; }

    public bool IsValid() => !string.IsNullOrEmpty(Placa) && !string.IsNullOrEmpty(Cor) && !string.IsNullOrEmpty(Status);
}

// Requisição para atualizar uma moto
public class UpdateMotoRequest
{
    public string? Placa { get; set; }
    public string? Cor { get; set; }
    public string? Status { get; set; }
    public int? TempoLimite { get; set; }

    public bool IsValid() => true; // Pode adicionar validações específicas
}

// Resposta para moto com links HATEOAS
public class MotoResponse : ResourceResponse
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public int TempoLimite { get; set; }

    public MotoResponse(Moto moto, Dictionary<string, Link> links)
    {
        Id = moto.Id;
        Placa = moto.Placa;
        Cor = moto.Cor;
        Status = moto.Status;
        DataEntrada = moto.DataEntrada;
        TempoLimite = moto.TempoLimite;
        Links = links;
    }
}

// Entidade: Colaborador
public class Colaborador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
}

// Requisição para criar colaborador
public class CreateColaboradorRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
    [Required(ErrorMessage = "Cargo é obrigatório")]
    public string Cargo { get; set; } = string.Empty;

    public bool IsValid() => !string.IsNullOrEmpty(Nome) && !string.IsNullOrEmpty(Cargo);
}

// Requisição para atualizar colaborador
public class UpdateColaboradorRequest
{
    public string? Nome { get; set; }
    public string? Cargo { get; set; }
}

// Resposta para colaborador com links HATEOAS
public class ColaboradorResponse : ResourceResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;

    public ColaboradorResponse(Colaborador colab, Dictionary<string, Link> links)
    {
        Id = colab.Id;
        Nome = colab.Nome;
        Cargo = colab.Cargo;
        Links = links;
    }
}

// Entidade: Alerta
public class Alerta
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int MotoId { get; set; }
    public DateTime DataAlerta { get; set; } = DateTime.Now;
}

// Requisição para criar alerta
public class CreateAlertaRequest
{
    [Required(ErrorMessage = "Descrição é obrigatória")]
    public string Descricao { get; set; } = string.Empty;
    [Required(ErrorMessage = "MotoId é obrigatório")]
    public int MotoId { get; set; }

    public bool IsValid() => !string.IsNullOrEmpty(Descricao) && MotoId > 0;
}

// Requisição para atualizar alerta
public class UpdateAlertaRequest
{
    public string? Descricao { get; set; }
    public int? MotoId { get; set; }
}

// Resposta para alerta com links HATEOAS
public class AlertaResponse : ResourceResponse
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int MotoId { get; set; }
    public DateTime DataAlerta { get; set; }

    public AlertaResponse(Alerta alerta, Dictionary<string, Link> links)
    {
        Id = alerta.Id;
        Descricao = alerta.Descricao;
        MotoId = alerta.MotoId;
        DataAlerta = alerta.DataAlerta;
        Links = links;
    }
}