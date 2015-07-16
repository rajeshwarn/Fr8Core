﻿using System.Linq;
using Data.Entities;
using Data.Interfaces;
using NUnit.Framework;
using StructureMap;

namespace DockyardTest.Entities
{
	[ TestFixture ]
	public class EnvelopeTests: BaseTest
	{
		[ Test ]
		[ Category( "Envelope" ) ]
		public void Envelope_Change_Status()
		{
			const string newStatus = "Created";
			const string updatedStatus = "Updated";
			using( var uow = ObjectFactory.GetInstance< IUnitOfWork >() )
			{
				uow.EnvelopeRepository.Add( new EnvelopeDO { Id = 1, Status = newStatus, DocusignEnvelopeId = "23" } );
				uow.SaveChanges();

				var createdEnvelope = uow.EnvelopeRepository.GetQuery().FirstOrDefault();
				Assert.NotNull( createdEnvelope );
				Assert.AreEqual( newStatus, createdEnvelope.Status );

				createdEnvelope.Status = updatedStatus;
				uow.SaveChanges();

				var updatedEnvelope = uow.EnvelopeRepository.GetQuery().FirstOrDefault();
				Assert.NotNull( updatedEnvelope );
				Assert.AreEqual( updatedStatus, updatedEnvelope.Status );
			}
		}
	}
}