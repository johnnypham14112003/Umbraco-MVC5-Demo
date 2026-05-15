using AutoMapper;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using Service.ViewModels;
using System;
using System.Collections.Generic;

namespace Service.Services
{
    public class MotorService : IMotorService
    {
        private readonly IMotorRepository _motorRepository;
        private readonly IMappingEngine _mapper;

        public MotorService(IMotorRepository motorRepository, IMappingEngine mapper)
        {
            _motorRepository = motorRepository;
            _mapper = mapper;
        }

        public IEnumerable<MotorViewModel> GetAll()
        {
            return _mapper.Map<IEnumerable<MotorViewModel>>(_motorRepository.GetAll());
        }

        public IEnumerable<MotorViewModel> Search(string keyword)
        {
            return string.IsNullOrWhiteSpace(keyword) == true ?
                GetAll() : _mapper.Map<IEnumerable<MotorViewModel>>(_motorRepository.GetByName(keyword));
        }

        public bool Add(MotorViewModel item)
        {
            if (item == null)
            {
                return false;
            }

            return _motorRepository.Add(_mapper.Map<Motor>(item));
        }

        public bool Delete(Guid id)
        {
            return _motorRepository.Delete(id);
        }

        public MotorViewModel GetDetail(Guid id)
        {
            var exist = _motorRepository.Get(id);
            return _mapper.Map<MotorViewModel>(exist);
        }

        public MotorViewModel UpdateImage(Guid id, string url)
        {
            return _mapper.Map<MotorViewModel>(_motorRepository.UpdateImage(id, url));
        }

        public bool Update(MotorViewModel item)
        {
            return _motorRepository.Update(_mapper.Map<Motor>(item));
        }
    }
}
