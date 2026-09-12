using AutoMapper;
using RL360.Domain.Entities;
using RL360.Domain.Radar;
using RL360.Shared.Dtos;

namespace RL360.Application.Mapping;

public sealed class PerfilRadar : Profile
{
    public PerfilRadar()
    {
        CreateMap<ResultadoFatorRadar, FatorRadarDto>()
            .ForMember(d => d.Fator, o => o.MapFrom(s => s.Fator.ToString()));

        CreateMap<ItemPlanoAcao, ItemPlanoAcaoDto>()
            .ForMember(d => d.Prioridade, o => o.MapFrom(s => s.Prioridade.ToString()));

        CreateMap<SnapshotRadar, SnapshotRadarDto>()
            .ForMember(d => d.StatusSaude, o => o.MapFrom(s => s.StatusSaude.ToString()));

        CreateMap<Venda, VendaDto>();

        CreateMap<EtapaFunil, EtapaFunilDto>();

        CreateMap<RegistroInadimplencia, InadimplenciaDto>()
            .ForMember(d => d.PerdaProjetada, o => o.MapFrom(s => s.PerdaProjetada));

        CreateMap<Gargalo, GargaloDto>()
            .ForMember(d => d.Area, o => o.MapFrom(s => s.Area.ToString()));

        CreateMap<Cliente, ClienteDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<MembroEquipe, MembroEquipeDto>();

        CreateMap<Meta, MetaDto>()
            .ForMember(d => d.PercentualAtingido, o => o.MapFrom(s => s.PercentualAtingido));

        CreateMap<Alerta, AlertaDto>()
            .ForMember(d => d.Severidade, o => o.MapFrom(s => s.Severidade.ToString()))
            .ForMember(d => d.FatorRelacionado, o => o.MapFrom(s => s.FatorRelacionado != null ? s.FatorRelacionado.ToString() : null))
            .ForMember(d => d.CriadoEmUtc, o => o.MapFrom(s => s.CriadoEmUtc));

        CreateMap<InsightInteligente, InsightInteligenteDto>();

        CreateMap<Usuario, UsuarioDto>()
            .ForMember(d => d.Perfil, o => o.MapFrom(s => s.Perfil.ToString()))
            .ForMember(d => d.NomeEmpresa, o => o.Ignore());
    }
}
